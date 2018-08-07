using System;

namespace MarginTrading.NotificationGenerator.Core.Settings
{
    public class NotificationsSettings
    {
        public bool EmailNotificationEnabled { get; set; }
        public TimeSpan InvocationTime { get; set; }
        public TradingReportFilter Filter { get; set; }
    }
}