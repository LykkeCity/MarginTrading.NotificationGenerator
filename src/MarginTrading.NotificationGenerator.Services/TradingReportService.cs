using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper.Mappers;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using MarginTrading.Backend.Contracts.Account;
using MarginTrading.Backend.Contracts.AccountHistory;
using MarginTrading.Backend.Contracts.TradeMonitoring;
using MarginTrading.NotificationGenerator.Core;
using MarginTrading.NotificationGenerator.Core.Domain;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.Core.Settings;
using Microsoft.Extensions.Internal;

namespace MarginTrading.NotificationGenerator.Services
{
    public class TradingReportService : ITradingReportService
    {
        private readonly IEmailService _emailService;
        private readonly IConvertService _convertService;
        private readonly ISystemClock _systemClock;
        private readonly ILog _log;
        private readonly TradingReportFilter _tradingReportFilter;

        private readonly IClientAccountClient _clientAccountClient;
        
        private readonly Backend.Contracts.IAccountsApi _accountsApi;
        private readonly Backend.Contracts.IAccountHistoryApi _accountHistoryApi;
        private readonly Backend.Contracts.ITradeMonitoringReadingApi _tradeMonitoringReadingApi;

        public TradingReportService(
            IEmailService emailService,
            IConvertService convertService,
            ISystemClock systemClock, 
            ILog log,
            TradingReportFilter tradingReportFilter,
            
            IClientAccountClient clientAccountClient,
            
            Backend.Contracts.IAccountsApi accountsApi,
            Backend.Contracts.IAccountHistoryApi accountHistoryApi,
            Backend.Contracts.ITradeMonitoringReadingApi tradeMonitoringReadingApi)
        {
            _emailService = emailService;
            _convertService = convertService;
            _systemClock = systemClock;
            _log = log;
            _tradingReportFilter = tradingReportFilter;

            _clientAccountClient = clientAccountClient;
            
            _accountsApi = accountsApi;
            _accountHistoryApi = accountHistoryApi;
            _tradeMonitoringReadingApi = tradeMonitoringReadingApi;
        }

        private bool LegalEntityPredicate(string legalEntity)
        {
            return string.IsNullOrWhiteSpace(_tradingReportFilter.LegalEntityRegex)
                   || (!string.IsNullOrWhiteSpace(legalEntity)
                       && Regex.IsMatch(legalEntity, _tradingReportFilter.LegalEntityRegex, RegexOptions.IgnoreCase));
        }

        public async Task PerformReporting()
        {
            var from = _systemClock.UtcNow.Date.AddMonths(-1);
            var to = _systemClock.UtcNow.Date;
            var reportMonth = _systemClock.UtcNow.Date.AddMonths(-1).Month;

            //gather data concurrently, await & filter & convert
            var pendingPositionsTask = _tradeMonitoringReadingApi.PendingOrders();
            var accountsTask = _accountsApi.GetAllAccounts();

            var accounts = (await accountsTask).Where(x => LegalEntityPredicate(x.LegalEntity))
                .Select(x => _convertService.Convert<DataReaderAccountBackendContract, Account>(x))
                .ToList();
            var clientIds = accounts.Select(x => x.ClientId).Distinct().ToArray();
            var accountIds = accounts.Select(x => x.Id).Distinct().ToHashSet();
            var accountClients = accounts.ToDictionary(x => x.Id, x => x.ClientId);
           
            var accountHistoryAggregate = new AccountHistoryResponse
            {
                Account = new AccountHistoryContract[0],
                OpenPositions = new OrderHistoryContract[0],
                PositionsHistory = new OrderHistoryContract[0]
            };
            foreach (var clientId in clientIds)
            {//TODO batch launcher on semaphores?
                var result = await _accountHistoryApi.ByTypes(new AccountHistoryRequest
                {
                    ClientId = clientId,
                    From = from, 
                    To = to,
                });
                accountHistoryAggregate.Account = accountHistoryAggregate.Account.Concat(result.Account).ToArray();
                accountHistoryAggregate.OpenPositions =
                    accountHistoryAggregate.OpenPositions.Concat(result.OpenPositions).ToArray();
                accountHistoryAggregate.PositionsHistory =
                    accountHistoryAggregate.PositionsHistory.Concat(result.PositionsHistory).ToArray();
            }
            
            var closedTrades = accountHistoryAggregate.PositionsHistory.Where(x => accountIds.Contains(x.AccountId))
                .Select(x => _convertService.Convert<OrderHistoryContract, OrderHistory>(x))
                .SetClientId(accountClients)
                .ToList();
            var openPositions = accountHistoryAggregate.OpenPositions.Where(x => accountIds.Contains(x.AccountId))
                .Select(x => _convertService.Convert<OrderHistoryContract, OrderHistory>(x))
                .SetClientId(accountClients)
                .ToList();
            var pendingPositions = (await pendingPositionsTask).Where(x => accountIds.Contains(x.AccountId))
                .Select(x => _convertService.Convert<OrderContract, OrderHistory>(x))
                .ToList();
            var accountTransactions = accountHistoryAggregate.Account.Where(x => accountIds.Contains(x.AccountId))
                .Select(x => _convertService.Convert<AccountHistoryContract, AccountHistory>(x))
                .ToList();

            //prepare notification models
            var notifications = clientIds.Select(x =>
                PrepareNotification(reportMonth, from, to, x, closedTrades, openPositions, pendingPositions,
                    accounts, accountTransactions)).ToList();

            //retrieve emails
            var emails = (await _clientAccountClient.GetClientsByIdsAsync(clientIds))
                .ToDictionary(x => x.Id, x => x.Email);
            //TODO for testing only
            emails = emails.Select(x => new KeyValuePair<string, string>(x.Key, "alexey.andreev@lykke.com"))
                .ToDictionary(x => x.Key, x => x.Value);

            await SendNotifications(notifications, emails);
        }

        private async Task SendNotifications(IEnumerable<MonthlyTradingNotification> notifications, 
            IReadOnlyDictionary<string, string> emails)
        {
            var failedNotifications = new Dictionary<MonthlyTradingNotification, Exception>();
            foreach (var notification in notifications)
            {
                try
                {
                    await _emailService.PrepareAndSendEmailAsync(emails[notification.ClientId],
                        $"Margin Trading - Monthly trading report for {notification.CurrentMonth}",
                        "MonthlyTradingReport",
                        notification);

                    await _log.WriteInfoAsync(nameof(NotificationGenerator), nameof(TradingReportService),
                        nameof(PerformReporting), notification.GetLogData(), _systemClock.UtcNow.DateTime);
                }
                catch (Exception ex)
                {
                    failedNotifications.Add(notification, ex);
                }
            }

            if (failedNotifications.Any())
            {
                //TODO handle failed notifications
                await _log.WriteWarningAsync(nameof(NotificationGenerator), nameof(TradingReportService),
                    nameof(PerformReporting), $"{failedNotifications.Count} notifications failed to be send.",
                    _systemClock.UtcNow.DateTime);
            }
        }

        private static MonthlyTradingNotification PrepareNotification(int reportMonth, 
            DateTime from, DateTime to, string clientId, 
            IReadOnlyList<OrderHistory> closedTrades, IReadOnlyList<OrderHistory> openPositions, 
            IReadOnlyList<OrderHistory> pendingPositions, IReadOnlyList<Account> accounts,
            IReadOnlyList<AccountHistory> accountTransactions)
        {
            var filteredAccounts = accounts.Where(x => x.ClientId == clientId)
                .Select(x =>
                {
                    x.AccountTransactions = accountTransactions.Where(at => at.ClientId == clientId
                                                                            && at.AccountId == x.Id)
                        .OrderByDescending(at => at.Date).ToList();
                    var firstTransaction = x.AccountTransactions.LastOrDefault();
                    x.InitialBalance = (firstTransaction?.Balance - firstTransaction?.Amount) ?? x.Balance;
                    return x;
                })
                .OrderByDescending(x => x.Balance).ThenBy(x => x.BaseAssetId).ToList();
            var accountIds = filteredAccounts.Select(x => x.Id).ToHashSet();
            return new MonthlyTradingNotification
            {
                CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(reportMonth),
                From = from.ToString("dd.MM.yyyy"),
                To = to.ToString("dd.MM.yyyy"),
                ClientId = clientId,
                ClosedTrades = closedTrades
                    .Where(x => x.ClientId == clientId && accountIds.Contains(x.AccountId))
                    .OrderByDescending(x => x.CloseDate).ToList(),
                OpenPositions = openPositions
                    .Where(x => x.ClientId == clientId && accountIds.Contains(x.AccountId))
                    .OrderByDescending(x => x.OpenDate).ToList(),
                PendingPositions = pendingPositions
                    .Where(x => x.ClientId == clientId && accountIds.Contains(x.AccountId))
                    .OrderByDescending(x => x.CreateDate).ToList(),
                Accounts = filteredAccounts,
            };
        }
    }
}