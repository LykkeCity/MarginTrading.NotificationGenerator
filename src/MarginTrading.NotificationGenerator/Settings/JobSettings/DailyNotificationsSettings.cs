﻿using System;
using MarginTrading.NotificationGenerator.Core.Settings;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class DailyNotificationsSettings
    {
        public bool EmailNotificationEnabled { get; set; }
        public TimeSpan InvocationTime { get; set; }
        public TradingReportFilter Filter { get; set; }
    }
}