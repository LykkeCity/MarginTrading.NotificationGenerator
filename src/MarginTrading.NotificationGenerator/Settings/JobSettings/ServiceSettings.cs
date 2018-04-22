using Lykke.SettingsReader.Attributes;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class ServiceSettings
    {
        public string Url { get; set; }
        [Optional]
        public string ApiKey { get; set; }
    }
}