using Lykke.SettingsReader.Attributes;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
        
        [AzureTableCheck]
        public string HistoryConnString { get; set; } //for overnight swap history
        
        //[SqlCheck]
        public string SqlReportsConnString { get; set; }
    }
}
