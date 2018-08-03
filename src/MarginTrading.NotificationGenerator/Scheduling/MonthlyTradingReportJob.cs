using System;
using FluentScheduler;
using MarginTrading.NotificationGenerator.Core;

namespace MarginTrading.NotificationGenerator.Scheduling
{
    public class MonthlyTradingReportJob : IJob, IDisposable
    {
        public MonthlyTradingReportJob()
        {
            
        }
        
        public void Execute()
        {
            MtServiceLocator.TradingReportService.PerformReportingMonthly();
        }

        public void Dispose()
        {
        }
    }
}