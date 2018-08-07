using Lykke.SettingsReader.Attributes;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class ServicesSettings
    {
        public ServiceSettings DataReader { get; set; }
        public ServiceSettings ClientAccount { get; set; }
        public ServiceSettings EmailSender { get; set; }
    }
}