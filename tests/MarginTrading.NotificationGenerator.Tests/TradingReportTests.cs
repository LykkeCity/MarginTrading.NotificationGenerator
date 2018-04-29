using System;
using Autofac;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.Core.Settings;
using MarginTrading.NotificationGenerator.Modules;
using MarginTrading.NotificationGenerator.Settings.JobSettings;
using MarginTrading.NotificationGenerator.Tests.Modules;
using NUnit.Framework;

namespace MarginTrading.NotificationGenerator.Tests
{
    [TestFixture]
    public class TradingReportTests
    {
        private IContainer Container { get; set; }
        private ITradingReportService TradingReportService { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var builder = new ContainerBuilder();

            var fakeSettings = new NotificationGeneratorSettings
            {
                MonthlyTradingReportSettings = new NotificationsSettings
                {
                    EmailNotificationEnabled = true,
                    Filter = new TradingReportFilter {LegalEntityRegex = "$LYKKECY^"},
                    InvocationTime = new TimeSpan(0, 0, 0),
                }, 
            };
            builder.RegisterModule(new JobModule(fakeSettings, null, null));
            builder.RegisterModule(new ExternalServiceMockModule());
            
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
        }
    }
}
