using System.Threading.Tasks;
using Autofac;
using Common;
using MarginTrading.NotificationGenerator.Contract;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface IMyRabbitPublisher : IStartable, IStopable
    {
        Task PublishAsync(MyPublishedMessage message);
    }
}