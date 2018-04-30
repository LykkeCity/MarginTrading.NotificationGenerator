using System.Threading.Tasks;
using Autofac;
using Common.Log;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.EmailSender;
using MarginTrading.Backend.Contracts;
using MarginTrading.NotificationGenerator.Tests.Mocks;
using Moq;

namespace MarginTrading.NotificationGenerator.Tests.Modules
{
    public class ExternalServiceMockModule : Module
    {
        private readonly ILog _log;

        internal readonly Mock<IEmailSender> EmailSenderMock;
        
        public ExternalServiceMockModule(ILog log)
        {
            _log = log;
            
            EmailSenderMock = new Mock<IEmailSender>();
            EmailSenderMock.Setup(x => x.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<EmailAddressee>()))
                .Returns(Task.CompletedTask);
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AccountsApiMock()).As<IAccountsApi>().SingleInstance();

            builder.RegisterInstance(new TradeMonitoringReadingApiMock()).As<ITradeMonitoringReadingApi>().SingleInstance();

            builder.RegisterInstance(new AccountHistoryApiMock()).As<IAccountHistoryApi>().SingleInstance();

            builder.RegisterInstance(new ClientAccountClientMock()).As<IClientAccountClient>().SingleInstance();

            builder.RegisterInstance(EmailSenderMock.Object).As<IEmailSender>().SingleInstance();
        }
    }
}