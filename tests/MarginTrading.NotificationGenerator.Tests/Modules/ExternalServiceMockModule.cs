using Autofac;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.EmailSender;
using MarginTrading.Backend.Contracts;
using MarginTrading.NotificationGenerator.Tests.Mocks;

namespace MarginTrading.NotificationGenerator.Tests.Modules
{
    public class ExternalServiceMockModule : Module
    {
        
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AccountsApiMock()).As<IAccountsApi>().SingleInstance();

            builder.RegisterInstance(new TradeMonitoringReadingApiMock()).As<ITradeMonitoringReadingApi>().SingleInstance();

            builder.RegisterInstance(new AccountHistoryApiMock()).As<IAccountHistoryApi>().SingleInstance();

            builder.RegisterInstance(new ClientAccountClientMock()).As<IClientAccountClient>().SingleInstance();

            builder.RegisterInstance(new EmailSenderMock()).As<IEmailSender>().SingleInstance();
        }
    }
}