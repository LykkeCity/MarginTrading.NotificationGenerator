using System;
using FluentScheduler;
using JetBrains.Annotations;
using MarginTrading.NotificationGenerator.Core;
using MarginTrading.NotificationGenerator.Core.Domain;

namespace MarginTrading.NotificationGenerator.Scheduling
{
    [UsedImplicitly]
    public class DailyTradingReportJob : IJob, IDisposable
    {
        public DailyTradingReportJob()
        {

        }

        public void Execute()
        {
            MtServiceLocator.TradingReportService.PerformReporting(OvernightSwapReportType.Daily);
        }

        public void Dispose()
        {
        }
    }
}