using Lykke.SettingsReader.Attributes;

namespace MarginTrading.NotificationGenerator.Settings.JobSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
#if (rabbitsub)

        public string ExchangeName { get; set; }
#endif
    }
}
