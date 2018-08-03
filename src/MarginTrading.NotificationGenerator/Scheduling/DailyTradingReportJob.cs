using System;
using FluentScheduler;
using MarginTrading.NotificationGenerator.Core;

namespace MarginTrading.NotificationGenerator.Scheduling
{
    public class DailyTradingReportJob : IJob, IDisposable
    {
        public DailyTradingReportJob()
        {

        }

        public void Execute()
        {
            MtServiceLocator.TradingReportService.PerformReporting();
        }

        public void Dispose()
        {
        }
    }
}