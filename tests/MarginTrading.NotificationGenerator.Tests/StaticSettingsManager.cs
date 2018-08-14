using System;
using System.Threading.Tasks;
using Lykke.SettingsReader;

namespace MarginTrading.NotificationGenerator.Tests
{
    public class StaticSettingsManager<T> : IReloadingManager<T>
    {
        public StaticSettingsManager(T currentValue)
        {
            HasLoaded = true;
            CurrentValue = currentValue;
        }

        public Task<T> Reload()
        {
            return Task.FromResult(CurrentValue);
        }

        public bool WasReloadedFrom(DateTime dateTime)
        {
            return false;
        }

        public bool HasLoaded { get; }
        public T CurrentValue { get; }
    }
}