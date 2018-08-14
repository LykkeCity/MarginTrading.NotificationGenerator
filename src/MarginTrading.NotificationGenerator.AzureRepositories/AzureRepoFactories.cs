using AzureStorage.Tables;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.NotificationGenerator.AzureRepositories.Entities;
using MarginTrading.NotificationGenerator.AzureRepositories.Repositories;

namespace MarginTrading.NotificationGenerator.AzureRepositories
{
    public class AzureRepoFactories
    {
        public static class MarginTrading
        {
            public static OvernightSwapHistoryRepository CreateOvernightSwapHistoryRepository(IReloadingManager<string> connString, ILog log)
            {
                return new OvernightSwapHistoryRepository(AzureTableStorage<OvernightSwapHistoryEntity>.Create(connString,
                    "OvernightSwapHistory", log));
            }
        }
    }
}