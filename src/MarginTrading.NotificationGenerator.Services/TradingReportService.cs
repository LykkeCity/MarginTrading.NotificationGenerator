using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MoreLinq;
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
using MarginTrading.NotificationGenerator.Core.Repositories;
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
        private readonly NotificationsSettings _monthlyNotificationsSettings;
        private readonly NotificationsSettings _dailyNotificationsSettings;

        private readonly IClientAccountClient _clientAccountClient;
        
        private readonly Backend.Contracts.IAccountsApi _accountsApi;
        private readonly Backend.Contracts.IAccountHistoryApi _accountHistoryApi;
        private readonly Backend.Contracts.ITradeMonitoringReadingApi _tradeMonitoringReadingApi;
        private readonly Backend.Contracts.IAssetPairsReadingApi _assetPairsReadingApi;

        private readonly IOvernightSwapHistoryRepository _overnightSwapHistoryRepository;

        public TradingReportService(
            IEmailService emailService,
            IConvertService convertService,
            ISystemClock systemClock, 
            ILog log,
            NotificationsSettings monthlyNotificationsSettings,//injected as named parameter
            NotificationsSettings dailyNotificationsSettings,//injected as named parameter
            
            IClientAccountClient clientAccountClient,
            
            Backend.Contracts.IAccountsApi accountsApi,
            Backend.Contracts.IAccountHistoryApi accountHistoryApi,
            Backend.Contracts.ITradeMonitoringReadingApi tradeMonitoringReadingApi,
            Backend.Contracts.IAssetPairsReadingApi assetPairsReadingApi,
            
            IOvernightSwapHistoryRepository overnightSwapHistoryRepository)
        {
            _emailService = emailService;
            _convertService = convertService;
            _systemClock = systemClock;
            _log = log;
            _monthlyNotificationsSettings = monthlyNotificationsSettings;
            _dailyNotificationsSettings = dailyNotificationsSettings;

            _clientAccountClient = clientAccountClient;
            
            _accountsApi = accountsApi;
            _accountHistoryApi = accountHistoryApi;
            _tradeMonitoringReadingApi = tradeMonitoringReadingApi;
            _assetPairsReadingApi = assetPairsReadingApi;

            _overnightSwapHistoryRepository = overnightSwapHistoryRepository;
        }

        private bool LegalEntityPredicate(OvernightSwapReportType reportType, string legalEntity)
        {
            var filter = reportType == OvernightSwapReportType.Daily 
                ? _dailyNotificationsSettings.Filter
                : _monthlyNotificationsSettings.Filter;
            
            return string.IsNullOrWhiteSpace(filter.LegalEntityRegex)
                   || (!string.IsNullOrWhiteSpace(legalEntity)
                       && Regex.IsMatch(legalEntity, filter.LegalEntityRegex, RegexOptions.IgnoreCase));
        }

        public async Task PerformReporting(OvernightSwapReportType reportType)
        {
            var from = reportType == OvernightSwapReportType.Daily 
                ? _systemClock.UtcNow.Date.AddDays(-1)
                : _systemClock.UtcNow.Date.AddMonths(-1);
            var to = _systemClock.UtcNow.Date;
            var reportPeriodIndex = from.Day;

            await _log.WriteInfoAsync(nameof(TradingReportService), nameof(PerformReporting),
                reportType == OvernightSwapReportType.Daily
                    ? $"Report invoked for {reportPeriodIndex} day, period from {from:s} to {to:s}"
                    : $"Report invoked for {reportPeriodIndex} month, period from {from:s} to {to:s}");

           (string[] clientIds, List<Account> accounts, List<OrderHistory> closedTrades, 
                   List<OrderHistory> openPositions, List<OrderHistory> pendingPositions, 
                   List<AccountHistory> accountTransactions) = await GetDataForNotifications(reportType, to, from);

            //prepare notification models
            var notifications = clientIds.Select(x =>
                PrepareNotification(reportType, from, to, x, closedTrades, openPositions,
                    pendingPositions, accounts, accountTransactions)).ToList();

            //retrieve emails
            var emails = (await _clientAccountClient.GetClientsByIdsAsync(clientIds))
                .ToDictionary(x => x.Id, x => x.Email);

            if (reportType == OvernightSwapReportType.Daily 
                ? _dailyNotificationsSettings.EmailNotificationEnabled
                : _monthlyNotificationsSettings.EmailNotificationEnabled)
            {
                await SendNotifications(reportType, notifications, emails);
            }
            else
            {
                await _log.WriteInfoAsync(nameof(TradingReportService), nameof(PerformReporting),
                    $"Email notifications are disabled for {reportType.ToString()} reports.");
            }
        }

        private async Task SendNotifications(OvernightSwapReportType reportType, 
            IEnumerable<PeriodicTradingNotification> notifications, IReadOnlyDictionary<string, string> emails)
        {
            var failedNotifications = new Dictionary<PeriodicTradingNotification, Exception>();
            Exception anyException = null;
            foreach (var notification in notifications)
            {
                if (reportType == OvernightSwapReportType.Monthly || 
                    (notification.ClosedTrades.Any() || notification.OpenPositions.Any() ||
                        notification.PendingPositions.Any()))
                {
                    try
                    {
                        await _emailService.PrepareAndSendEmailAsync(emails[notification.ClientId],
                            "Margin Trading - " + (reportType == OvernightSwapReportType.Daily ? "Daily" : "Monthly")
                                +$" trading report for {notification.CurrentPeriod}",
                            reportType == OvernightSwapReportType.Daily ? "DailyTradingReport" : "MonthlyTradingReport",
                            notification);

                        await _log.WriteInfoAsync(nameof(NotificationGenerator), nameof(TradingReportService),
                            nameof(PerformReporting), notification.GetLogData(), _systemClock.UtcNow.DateTime);
                    }
                    catch (Exception ex)
                    {
                        anyException = ex;
                        failedNotifications.Add(notification, ex);
                    }
                }
            }

            if (failedNotifications.Any())
            {
                //TODO handle failed notifications
                await _log.WriteWarningAsync(nameof(NotificationGenerator), nameof(TradingReportService),
                    nameof(PerformReporting), $"{failedNotifications.Count} notifications failed to be send.",
                    anyException, _systemClock.UtcNow.DateTime);
            }
        }

        private static PeriodicTradingNotification PrepareNotification(OvernightSwapReportType reportType, 
            DateTime from, DateTime to, string clientId, 
            IReadOnlyCollection<OrderHistory> closedTrades, IReadOnlyCollection<OrderHistory> openPositions, 
            IEnumerable<OrderHistory> pendingPositions, IEnumerable<Account> accounts,
            IEnumerable<AccountHistory> accountTransactions)
        {
            var orderInstruments = closedTrades.Concat(openPositions).ToDictionary(x => x.Id, x => x.Instrument);
            var closedTradesPnls = closedTrades.GroupBy(x => x.AccountId).ToDictionary(x => x.Key, x => x.Sum(ct => ct.PnL));
            var floatingPnls = openPositions.GroupBy(x => x.AccountId).ToDictionary(x => x.Key, x => x.Sum(ct => ct.PnL));
            
            var filteredAccounts = accounts.Where(x => x.ClientId == clientId)
                .Select(x =>
                {
                    x.AccountTransactions = accountTransactions.Where(at => at.ClientId == clientId
                                                                            && at.AccountId == x.Id)
                        .Select(at =>
                        {
                            at.Comment = !string.IsNullOrEmpty(at.OrderId) 
                                         && orderInstruments.TryGetValue(at.OrderId, out var instrument)
                                ? $"{instrument}: {at.OrderId}"
                                : at.Comment;
                            return at;
                        })
                        .OrderByDescending(at => at.Date).ToList();
    
                    if (closedTradesPnls.TryGetValue(x.Id, out var closedTradesPnl))
                        x.ClosedTradesPnl = closedTradesPnl;
                    if (floatingPnls.TryGetValue(x.Id, out var floatingPnl))
                        x.FloatingPnl = floatingPnl;
                    x.CashTransactions = x.AccountTransactions.Sum(at => at.Amount);
                    x.Equity = x.Balance - x.FloatingPnl;
                    x.ChangeInBalance = x.ClosedTradesPnl + x.CashTransactions;
                    x.AvailableMargin = x.MarginRequirements - x.Equity;
                    
                    return x;
                })
                .OrderByDescending(x => x.Balance).ThenBy(x => x.BaseAssetId).ToList();
            
            var accountIds = Enumerable.ToHashSet(filteredAccounts.Select(x => x.Id));
            return new PeriodicTradingNotification
            {
                CurrentPeriod = reportType == OvernightSwapReportType.Daily 
                    ? from.ToString("dd.MM.yyyy")
                    : from.ToString("MM.yyyy"),
                From = $"{@from:dd.MM.yyyy mm:ss}",
                To = $"{to.AddMinutes(-1):dd.MM.yyyy mm:ss}",
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
                ReportType = reportType,
            };
        }

        private async Task<(string[], List<Account>, List<OrderHistory>, List<OrderHistory>, List<OrderHistory>, List<AccountHistory>)>
            GetDataForNotifications(OvernightSwapReportType reportType, DateTime to, DateTime from)
        {
            //gather data concurrently, await & filter & convert
            var pendingPositionsTask = _tradeMonitoringReadingApi.PendingOrders();
            var accountsTask = _accountsApi.GetAllAccounts();
            var accountsStatsTask = _accountsApi.GetAllAccountStats();
            var assetPairsTask = _assetPairsReadingApi.List();

            var accountsUsedMargin = (await accountsStatsTask).ToDictionary(x => x.AccountId, x => x.UsedMargin);
            var accounts = (await accountsTask).Where(x => LegalEntityPredicate(reportType, x.LegalEntity))
                .Select(x =>
                {
                    var account = _convertService.Convert<DataReaderAccountBackendContract, Account>(x);
                    if (accountsUsedMargin.TryGetValue(x.Id, out var usedMargin))
                    {
                        account.MarginRequirements = usedMargin;
                    }
                    else
                    {
                        _log.WriteWarningAsync(nameof(TradingReportService), nameof(GetDataForNotifications),
                            $"No corresponding account in GetAllAccountStats for account {x.Id}").GetAwaiter().GetResult();
                    }

                    return account;
                })
                .ToList();
            var clientIds = accounts.Select(x => x.ClientId).Distinct().ToArray();
            var accountClients = accounts.ToDictionary(x => x.Id, x => x.ClientId);
            var assetPairs = await assetPairsTask;
            var assetPairNames = assetPairs.ToDictionary(x => x.Id, x => x.Name);
            var assetPairAccuracy = assetPairs.ToDictionary(x => x.Id, x => x.Accuracy);

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

            //grab all swaps history here, and put it to closedTrades and openPositions
            var swapsSumByPositionId = (await _overnightSwapHistoryRepository.GetAsync(
                    from: accountHistoryAggregate.PositionsHistory.Concat(accountHistoryAggregate.OpenPositions)
                        .Select(x => x.OpenDate).Min(), 
                    to: null)) 
                .GroupBy(x => x.OpenOrderId).ToDictionary(x => x.Key, x => x.Sum(i => i.Value));
            
            //prepare output data
            var closedTrades = accountHistoryAggregate.PositionsHistory.Where(x => accountClients.ContainsKey(x.AccountId))
                .Select(x => Convert(assetPairAccuracy, _convertService.Convert<OrderHistoryContract, OrderHistory>(x)))
                .DistinctBy(x => x.Id)
                .SetClientId(accountClients)
                .SetInstrumentName(assetPairNames)
                .SetSwaps(swapsSumByPositionId)
                .ToList();
            var openPositions = accountHistoryAggregate.OpenPositions.Where(x => accountClients.ContainsKey(x.AccountId))
                .Select(x => Convert(assetPairAccuracy, _convertService.Convert<OrderHistoryContract, OrderHistory>(x)))
                .SetClientId(accountClients)
                .SetInstrumentName(assetPairNames)
                .SetSwaps(swapsSumByPositionId)
                .ToList();
            var pendingPositions = (await pendingPositionsTask).Where(x => accountClients.ContainsKey(x.AccountId))
                .Select(x => Convert(assetPairAccuracy, _convertService.Convert<OrderContract, OrderHistory>(x)))
                .SetInstrumentName(assetPairNames)
                .ToList();

            var accountTransactions = accountHistoryAggregate.Account.Where(x => accountClients.ContainsKey(x.AccountId))
                .Select(x => _convertService.Convert<AccountHistoryContract, AccountHistory>(x))
                .ToList();

            return (clientIds, accounts, closedTrades, openPositions, pendingPositions, accountTransactions);

        }

        private static OrderHistory Convert(IReadOnlyDictionary<string, int> accuracy, OrderHistory orderHistory)
        {
            return orderHistory.ApplyPriceAccuracy(accuracy[orderHistory.Instrument]);
        }
    }
}