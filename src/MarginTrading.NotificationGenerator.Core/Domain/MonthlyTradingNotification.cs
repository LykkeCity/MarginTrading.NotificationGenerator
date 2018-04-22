using System.Collections.Generic;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class MonthlyTradingNotification
    {
        public string CurrentMonth { get; set; }
        public string ClientId { get; set; }

        public List<TradingPosition> ClosedTrades { get; set; }
        public List<Order> OpenPositions { get; set; }
        public List<Order> PendingPositions { get; set; }
        public List<Account> Accounts { get; set; }

        public string GetLogData()
        {
            return $"{nameof(MonthlyTradingNotification)} for {ClientId} sent. Closed trades: {ClosedTrades.Count}, open positions: {OpenPositions.Count}, pending orders: {PendingPositions.Count}, accounts: {Accounts.Count}.";
        }
    }
}