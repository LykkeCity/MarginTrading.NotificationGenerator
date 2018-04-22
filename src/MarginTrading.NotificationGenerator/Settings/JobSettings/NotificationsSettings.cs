using System;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class NotificationsSettings
    {
        public bool EmailNotificationEnabled { get; set; }
        public TimeSpan InvocationTime { get; set; }
    }
}