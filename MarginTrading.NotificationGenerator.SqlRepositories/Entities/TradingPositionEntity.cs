using System;

namespace MarginTrading.NotificationGenerator.SqlRepositories.Entities
{
    public class TradingPositionEntity
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }

        public string TakerPositionId { get; set; }
        public string TakerCounterpartyId { get; set; }
        public string TakerAccountId { get; set; }
        public string TakerAccountAssetId { get; set; }
        public string PositionStatus { get; set; }
        public string CoreSymbol { get; set; }
        public string CoreSide { get; set; }
        public double Volume { get; set; }
        public double? MatchedVolume { get; set; }
        public double? MatchedCloseVolume { get; set; }
        public double? OpenPrice { get; set; }
        public double? ClosePrice { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string CloseReason { get; set; }
        public double? OpenCommission { get; set; }
        public double? CloseCommission { get; set; }
        public double SwapCommission { get; set; }
        public string FillType { get; set; }
        public string MatchingEngineMode { get; set; }

        public double? Fpl { get; set; }
        public double? PnL { get; set; }
        public double? PnlInUsd { get; set; }
        public double? VolumeUsdAtOpen { get; set; }
        public double? VolumeUsdAtClose { get; set; }
        public string EquivalentAsset { get; set; }
        public double OpenPriceEquivalent{ get; set; }
        public double ClosePriceEquivalent { get; set; }

        private string GetId()
        {
            return $"{TakerCounterpartyId}_{TakerPositionId}";
        }
    }
}