using System.Threading.Tasks;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}