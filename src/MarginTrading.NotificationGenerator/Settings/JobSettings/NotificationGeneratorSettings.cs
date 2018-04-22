using MarginTrading.NotificationGenerator.Settings.SlackNotifications;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class NotificationGeneratorSettings
    {
        public DbSettings Db { get; set; }
        
        public ServicesSettings Services { get; set; }
        
        public NotificationsSettings MonthlyTradingReportSettings { get; set; } 
    }
}
