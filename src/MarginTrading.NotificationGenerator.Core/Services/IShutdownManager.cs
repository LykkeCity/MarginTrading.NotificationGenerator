﻿using System.Threading.Tasks;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
