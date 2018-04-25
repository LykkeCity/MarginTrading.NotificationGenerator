using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using MarginTrading.Backend.Contracts.Account;
using MarginTrading.Backend.Contracts.AccountHistory;
using MarginTrading.Backend.Contracts.TradeMonitoring;
using MarginTrading.NotificationGenerator.Core;
using MarginTrading.NotificationGenerator.Core.Domain;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.SqlRepositories;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Internal;

namespace MarginTrading.NotificationGenerator.Services
{
    public class TradingReportService : ITradingReportService
    {
        private readonly IEmailService _emailService;
        private readonly IConvertService _convertService;
        private readonly ISystemClock _systemClock;
        private readonly ILog _log;

        private readonly IClientAccountClient _clientAccountClient;
        
        private readonly Backend.Contracts.IAccountsApi _accountsApi;
        private readonly Backend.Contracts.IAccountHistoryApi _accountHistoryApi;
        private readonly Backend.Contracts.ITradeMonitoringReadingApi _tradeMonitoringReadingApi;

        public TradingReportService(
            IEmailService emailService,
            IConvertService convertService,
            ISystemClock systemClock, 
            ILog log, 
            
            IClientAccountClient clientAccountClient,
            
            Backend.Contracts.IAccountsApi accountsApi,
            Backend.Contracts.IAccountHistoryApi accountHistoryApi,
            Backend.Contracts.ITradeMonitoringReadingApi tradeMonitoringReadingApi)
        {
            _emailService = emailService;
            _convertService = convertService;
            _systemClock = systemClock;
            _log = log;

            _clientAccountClient = clientAccountClient;
            
            _accountsApi = accountsApi;
            _accountHistoryApi = accountHistoryApi;
            _tradeMonitoringReadingApi = tradeMonitoringReadingApi;
        }

        public async Task PerformReporting()
        {
            var from = _systemClock.UtcNow.Date.AddMonths(-1);
            var to = _systemClock.UtcNow.Date;
            var reportMonth = _systemClock.UtcNow.Date.AddMonths(-1).Month;

            //gather data concurrently
            var accountHistoryAggregateTask = _accountHistoryApi.ByTypes(new AccountHistoryRequest { From = from, To = to, });
            var pendingPositionsTask = _tradeMonitoringReadingApi.PendingOrders();
            var accountsTask = _accountsApi.GetAllAccounts();
            
            //await & convert
            var accountHistoryAggregate = await accountHistoryAggregateTask;
            var closedTrades = accountHistoryAggregate.PositionsHistory
                .Select(x => _convertService.Convert<OrderHistoryContract, OrderHistory>(x))
                .ToList();
            var openPositions = accountHistoryAggregate.OpenPositions
                .Select(x => _convertService.Convert<OrderHistoryContract, OrderHistory>(x))
                .ToList();
            var pendingPositions = (await pendingPositionsTask)
                .Select(x => _convertService.Convert<OrderContract, OrderHistory>(x))
                .ToList();
            var accounts = (await accountsTask)
                .Select(x => _convertService.Convert<DataReaderAccountBackendContract, Account>(x))
                .ToList();
            var accountTransactions = accountHistoryAggregate.Account
                .Select(x => _convertService.Convert<AccountHistoryContract, AccountHistory>(x))
                .ToList();
            var clients = accounts.Select(x => x.ClientId).Distinct().ToArray();

            //prepare notification models
            var notifications = clients.Select(x =>
                PrepareNotification(reportMonth, from, to, x, closedTrades, openPositions, pendingPositions,
                    accounts, accountTransactions)).ToList();

            //retrieve emails
            var emails = (await _clientAccountClient.GetClientsByIdsAsync(clients))
                .ToDictionary(x => x.Id, x => x.Email);

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
            IEnumerable<OrderHistory> closedTrades, IEnumerable<OrderHistory> openPositions, 
            IEnumerable<OrderHistory> pendingPositions, IEnumerable<Account> accounts,
            IEnumerable<AccountHistory> accountTransactions)
        {
            return new MonthlyTradingNotification
            {
                CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(reportMonth),
                From = from.ToString("dd.MM.yyyy"),
                To = to.ToString("dd.MM.yyyy"),
                ClientId = clientId,
                //TODO push filter to settings + resolve legal entity via account
                ClosedTrades = closedTrades.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.CloseDate).ToList(), 
                OpenPositions = openPositions.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.OpenDate).ToList(), 
                PendingPositions = pendingPositions.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.CreateDate).ToList(), 
                Accounts = accounts.Where(x => x.ClientId == clientId)
                    .Select(x =>
                    {
                        x.AccountTransactions = accountTransactions.Where(at => at.ClientId == clientId
                                                                                && at.AccountId == x.Id
                                                                                && x.LegalEntity == "LYKKECY")
                            .OrderByDescending(at => at.Date).ToList();
                        x.InitialBalance = x.AccountTransactions.LastOrDefault()?.Balance ?? x.Balance;
                        return x;
                    })
                    .OrderByDescending(x => x.Balance).ThenBy(x => x.BaseAssetId).ToList(),
            };
        }
    }
}