using System;
using FluentScheduler;
using JetBrains.Annotations;
using MarginTrading.NotificationGenerator.Core;
using MarginTrading.NotificationGenerator.Core.Domain;

namespace MarginTrading.NotificationGenerator.Scheduling
{
    [UsedImplicitly]
    public class MonthlyTradingReportJob : IJob, IDisposable
    {
        public MonthlyTradingReportJob()
        {
            
        }
        
        public void Execute()
        {
            MtServiceLocator.TradingReportService.PerformReporting(OvernightSwapReportType.Monthly);
        }

        public void Dispose()
        {
        }
    }
}