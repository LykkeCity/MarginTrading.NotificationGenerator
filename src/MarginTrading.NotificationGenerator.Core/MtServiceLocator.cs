using System.Dynamic;
using MarginTrading.NotificationGenerator.Core.Services;
using Microsoft.Extensions.Internal;

namespace MarginTrading.NotificationGenerator.Core
{
    public static class MtServiceLocator
    {
        public static ITradingReportService TradingReportService { get; set; }
    }
}