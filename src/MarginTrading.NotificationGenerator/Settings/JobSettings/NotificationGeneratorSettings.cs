﻿using MarginTrading.NotificationGenerator.Core.Settings;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class NotificationGeneratorSettings
    {
        public DbSettings Db { get; set; }
        
        public ServicesSettings Services { get; set; }
        
        public NotificationsSettings MonthlyTradingReportSettings { get; set; } 

        public NotificationsSettings DailyTradingReportSettings { get; set; }
    }
}
