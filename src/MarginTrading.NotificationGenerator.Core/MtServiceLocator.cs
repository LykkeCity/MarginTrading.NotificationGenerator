using MarginTrading.NotificationGenerator.Core.Services;

namespace MarginTrading.NotificationGenerator.Core
{
    public static class MtServiceLocator
    {
        public static ITradingReportService TradingReportService { get; set; }
    }
}