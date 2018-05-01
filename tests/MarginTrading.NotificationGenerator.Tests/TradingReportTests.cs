using System;
using System.Linq;
using Autofac;
using Common.Log;
using Lykke.Service.EmailSender;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.Core.Settings;
using MarginTrading.NotificationGenerator.Modules;
using MarginTrading.NotificationGenerator.Settings.JobSettings;
using MarginTrading.NotificationGenerator.Tests.Mocks;
using MarginTrading.NotificationGenerator.Tests.Modules;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;

namespace MarginTrading.NotificationGenerator.Tests
{
    [TestFixture]
    public class TradingReportTests
    {
        private IContainer Container { get; set; }
        private ITradingReportService TradingReportService { get; set; }
        private ILog _log;

        private Mock<IEmailSender> EmailSenderMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _log = new Mock<ILog>().Object;
            var hostingEnvMock = new Mock<IHostingEnvironment>();
            hostingEnvMock.Setup(x => x.ContentRootPath).Returns("../../../../../src/MarginTrading.NotificationGenerator/");
            
            var builder = new ContainerBuilder();

            var fakeSettings = new NotificationGeneratorSettings
            {
                MonthlyTradingReportSettings = new NotificationsSettings
                {
                    EmailNotificationEnabled = true,
                    Filter = new TradingReportFilter {LegalEntityRegex = "^LYKKECY$"},
                    InvocationTime = new TimeSpan(0, 0, 0),
                }, 
            };
            builder.RegisterModule(new JobModule(fakeSettings, _log, hostingEnvMock.Object));
            var externalMockModule = new ExternalServiceMockModule(_log);
            EmailSenderMock = externalMockModule.EmailSenderMock;
            builder.RegisterModule(externalMockModule);
            
            Container = builder.Build();

            TradingReportService = Container.Resolve<ITradingReportService>();
        }

        [SetUp]
        public void SetUp()
        {
            
        }
        
        [Test]
        public void Simple_Success_Scenario()
        {
            TradingReportService.PerformReporting();
/*
            var clientAccounts = ClientAccountClientMock.GetClientAccounts().ToList();
            EmailSenderMock.Verify(x => x.SendAsync(It.IsAny<EmailMessage>(),
                It.Is<EmailAddressee>(a =>
                    a.EmailAddress == clientAccounts.First(c => c.Id == "c1").Email)), Times.Once);
            EmailSenderMock.Verify(x => x.SendAsync(It.IsAny<EmailMessage>(),
                It.Is<EmailAddressee>(a =>
                    a.EmailAddress == clientAccounts.First(c => c.Id == "c2").Email)), Times.Never);*/
        }
    }
}
