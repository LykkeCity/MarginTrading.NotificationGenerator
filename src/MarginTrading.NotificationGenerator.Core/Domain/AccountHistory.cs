using System;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class AccountHistory
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }

        public string AccountId { get; set; }

        public string ClientId { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string Comment { get; set; }

        public string Type { get; set; }

        public string OrderId { get; set; }

        public string LegalEntity { get; set; }
    }
}