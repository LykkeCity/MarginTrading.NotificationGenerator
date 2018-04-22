using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using MarginTrading.Backend.Contracts.Account;
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
        private readonly Backend.Contracts.ITradeMonitoringReadingApi _tradeMonitoringReadingApi;
        private readonly ITradingPositionSqlRepository _tradingPositionSqlRepository;

        public TradingReportService(
            IEmailService emailService,
            IConvertService convertService,
            ISystemClock systemClock, 
            ILog log, 
            
            IClientAccountClient clientAccountClient,
            
            Backend.Contracts.IAccountsApi accountsApi,
            Backend.Contracts.ITradeMonitoringReadingApi tradeMonitoringReadingApi,
            ITradingPositionSqlRepository tradingPositionSqlRepository)
        {
            _emailService = emailService;
            _convertService = convertService;
            _systemClock = systemClock;
            _log = log;

            _clientAccountClient = clientAccountClient;
            
            _accountsApi = accountsApi;
            _tradeMonitoringReadingApi = tradeMonitoringReadingApi;
            _tradingPositionSqlRepository = tradingPositionSqlRepository;
        }

        public async Task PerformReporting()
        {
            var from = _systemClock.UtcNow.Date.AddMonths(-1);
            var to = _systemClock.UtcNow.Date;
            var reportMonth = _systemClock.UtcNow.Date.AddMonths(-1).Month;

            //gather data concurrently
            var closedTradesTask = _tradingPositionSqlRepository.GetClosedFromPeriod(from, to);
            var openPositionsTask = _tradeMonitoringReadingApi.OpenPositions();
            var pendingPositionsTask = _tradeMonitoringReadingApi.PendingOrders();
            var accountsTask = _accountsApi.GetAllAccounts();
            
            //await & convert
            var closedTrades = (await closedTradesTask).ToList();
            var openPositions = (await openPositionsTask)
                .Select(x => _convertService.Convert<OrderContract, Order>(x))
                .ToList();
            var pendingPositions = (await pendingPositionsTask)
                .Select(x => _convertService.Convert<OrderContract, Order>(x))
                .ToList();
            var accounts = (await accountsTask)
                .Select(x => _convertService.Convert<DataReaderAccountBackendContract, Account>(x))
                .ToList();
            var clients = accounts.Select(x => x.ClientId).Distinct().ToArray();

            //prepare notification models
            var notifications = clients.Select(x =>
                PrepareNotification(reportMonth, x, closedTrades, openPositions, pendingPositions, accounts))
                .ToList();

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

        private static MonthlyTradingNotification PrepareNotification(int reportMonth, string clientId, 
            IEnumerable<TradingPosition> closedTrades, IEnumerable<Order> openPositions, 
            IEnumerable<Order> pendingPositions, IEnumerable<Account> accounts)
        {
            return new MonthlyTradingNotification
            {
                CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(reportMonth),
                ClientId = clientId,
                
                ClosedTrades = closedTrades.Where(x => x.TakerCounterpartyId == clientId)
                    .OrderByDescending(x => x.CloseDate).ToList(), 
                OpenPositions = openPositions.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.OpenDate).ToList(), 
                PendingPositions = pendingPositions.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.CreateDate).ToList(), 
                Accounts = accounts.Where(x => x.ClientId == clientId)
                    .OrderByDescending(x => x.Balance).ThenBy(x => x.BaseAssetId).ToList(),
            };
        }
    }
}