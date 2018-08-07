using System.Collections.Generic;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class PeriodicTradingNotification
    {
        public string CurrentPeriod { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ClientId { get; set; }

        public List<OrderHistory> ClosedTrades { get; set; }
        public List<OrderHistory> OpenPositions { get; set; }
        public List<OrderHistory> PendingPositions { get; set; }
        public List<Account> Accounts { get; set; }
        
        public OvernightSwapReportType ReportType { get; set; }

        public string GetLogData()
        {
            return ReportType == OvernightSwapReportType.Daily ? $"Daily" : $"Monthly"
                + $" trading notification for {ClientId} sent. Closed trades: {ClosedTrades.Count}, open positions: {OpenPositions.Count}, pending orders: {PendingPositions.Count}, accounts: {Accounts.Count}.";   
        }
    }
}