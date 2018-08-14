using System.Collections.Generic;
using JetBrains.Annotations;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    [UsedImplicitly]
    public class Account
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public string BaseAssetId { get; set; }

        public string LegalEntity { get; set; }

        public List<AccountHistory> AccountTransactions { get; set; } = new List<AccountHistory>();

        #region Summary

        public decimal ClosedTradesPnl { get; set; }

        public decimal FloatingPnl { get; set; }

        public decimal CashTransactions { get; set; }

        public decimal Equity { get; set; }

        public decimal ChangeInBalance { get; set; }

        public decimal MarginRequirements { get; set; }

        public decimal Balance { get; set; }

        public decimal AvailableMargin { get; set; }

        #endregion Summary
    }
}