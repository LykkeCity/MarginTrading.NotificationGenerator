using System.Collections.Generic;
using System.Linq;
using MarginTrading.NotificationGenerator.Core.Domain;

namespace MarginTrading.NotificationGenerator.Core
{
    public static class OrderHistoryExtensions
    {
        public static IEnumerable<OrderHistory> SetClientId(this IEnumerable<OrderHistory> orderHistory, 
            Dictionary<string, string> accountClients)
        {
            return orderHistory.Select(x =>
            {
                x.ClientId = accountClients[x.AccountId];
                return x;
            });
        } 
        
        public static IEnumerable<OrderHistory> SetInstrumentName(this IEnumerable<OrderHistory> orderHistory, 
            Dictionary<string, string> instruments)
        {
            return orderHistory.Select(x =>
            {
                x.Instrument = instruments.TryGetValue(x.Instrument, out var instrumentName) 
                    ? instrumentName
                    : x.Instrument;
                return x;
            });
        } 
        
        public static IEnumerable<OrderHistory> SetSwaps(this IEnumerable<OrderHistory> orderHistory, 
            Dictionary<string, decimal> swapsSum)
        {
            return orderHistory.Select(x =>
            {
                x.Swap = swapsSum.TryGetValue(x.Id, out var swaps)
                    ? swaps
                    : 0;
                return x;
            });
        } 
    }
}