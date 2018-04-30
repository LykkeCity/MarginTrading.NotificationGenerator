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
    }
}