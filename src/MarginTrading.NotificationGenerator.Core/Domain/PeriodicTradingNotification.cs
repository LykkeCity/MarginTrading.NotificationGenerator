using System.Collections.Generic;
using System.Linq;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class PeriodicTradingNotification
    {
        public string CurrentPeriod { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ClientId { get; set; }

        public List<Account> Accounts { get; set; }
        
        public OvernightSwapReportType ReportType { get; set; }

        public string GetLogData()
        {
            return ReportType == OvernightSwapReportType.Daily ? "Daily" : "Monthly"
                + $" trading notification for {ClientId} sent. Closed trades: {Accounts.Sum(x => x.ClosedTrades.Count)}, open positions: {Accounts.Sum(x => x.OpenPositions.Count)}, pending orders: {Accounts.Sum(x => x.PendingPositions.Count)}, accounts: {Accounts.Count}.";   
        }
    }
}