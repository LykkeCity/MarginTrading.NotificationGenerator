using MarginTrading.NotificationGenerator.Settings.JobSettings;
using MarginTrading.NotificationGenerator.Settings.SlackNotifications;

namespace MarginTrading.NotificationGenerator.Settings
{
    public class AppSettings
    {
        public NotificationGeneratorSettings MtNotificationGeneratorSettings { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
