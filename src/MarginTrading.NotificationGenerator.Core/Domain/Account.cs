using System.Collections.Generic;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class Account
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public string BaseAssetId { get; set; }

        public decimal Balance { get; set; }

        public string LegalEntity { get; set; }

        public List<AccountHistory> AccountTransactions { get; set; } = new List<AccountHistory>();
    }
}