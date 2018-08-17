using System;

namespace MarginTrading.NotificationGenerator.Core.Domain
{
    public class OrderHistory
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public string AccountId { get; set; }

        public string AccountAssetId { get; set; }

        public string Instrument { get; set; }

        public string Type { get; set; }

        public decimal ExpectedOpenPrice { get; set; }

        public decimal OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }
        
        public DateTime CreateDate { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }
        
        public string CloseReason { get; set; }

        public decimal Volume { get; set; }

        public decimal? TakeProfit { get; set; }

        public decimal? StopLoss { get; set; }
        
        public decimal Swap { get; set; }
        
        public decimal PnL { get; set; }

        public OrderHistory ApplyPriceAccuracy(int accuracy)
        {
            ExpectedOpenPrice = Math.Round(ExpectedOpenPrice, accuracy);
            OpenPrice = Math.Round(OpenPrice, accuracy);
            ClosePrice = Math.Round(ClosePrice, accuracy);
            if(TakeProfit.HasValue)
                TakeProfit = Math.Round(TakeProfit.Value, accuracy);
            if(StopLoss.HasValue)
                StopLoss = Math.Round(StopLoss.Value, accuracy);
            return this;
        }
    }
}