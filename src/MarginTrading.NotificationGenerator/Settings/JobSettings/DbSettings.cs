using Lykke.SettingsReader.Attributes;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
        
        //[SqlCheck]
        public string SqlReportsConnString { get; set; }
    }
}
