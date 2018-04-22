using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.HttpClientGenerator;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.EmailSender;
using Lykke.SettingsReader;
using MarginTrading.Backend.Contracts.DataReaderClient;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.Services;
using MarginTrading.NotificationGenerator.Settings.JobSettings;
using MarginTrading.NotificationGenerator.SqlRepositories;
using MarginTrading.NotificationGenerator.SqlRepositories.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace MarginTrading.NotificationGenerator.Modules
{
    public class JobModule : Module
    {
        private readonly IHostingEnvironment _environment;
        private readonly IReloadingManager<NotificationGeneratorSettings> _settingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<NotificationGeneratorSettings> settingsManager, ILog log, IHostingEnvironment environment)
        {
            _log = log;
            _environment = environment;
            _settingsManager = settingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log).As<ILog>().SingleInstance();

            builder.RegisterType<HealthService>().As<IHealthService>().SingleInstance();

            builder.RegisterType<StartupManager>().As<IStartupManager>();

            builder.RegisterType<ShutdownManager>().As<IShutdownManager>();

            // TODO: Add your dependencies here
            
            builder.RegisterInstance(_settingsManager).As<IReloadingManager<NotificationGeneratorSettings>>().SingleInstance();
            
            builder.RegisterType<SystemClock>().As<ISystemClock>().SingleInstance();
            
            builder.RegisterType<EmailService>().As<IEmailService>().SingleInstance();
            
            builder.Register<ITemplateGenerator>(ctx =>
                new MustacheTemplateGenerator(_environment, "EmailTemplates")
            ).SingleInstance();
            
            builder.RegisterType<TradingReportService>().As<ITradingReportService>().SingleInstance();
            
            builder.RegisterType<ConvertService>().As<IConvertService>().SingleInstance();

            RegisterExternalServices(builder);

            RegisterSqlRepositories(builder);

            builder.Populate(_services);
        }

        private void RegisterSqlRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<TradingPositionSqlRepository>()
                .As<ITradingPositionSqlRepository>()
                .WithParameter(new NamedParameter("connectionString", 
                    _settingsManager.Nested(x => x.Db.SqlReportsConnString).CurrentValue))
                .SingleInstance();
        }

        private void RegisterExternalServices(ContainerBuilder builder)
        {
            var dataReaderHttpGenerator = HttpClientGenerator
                .BuildForUrl(_settingsManager.Nested(x => x.Services.DataReader.Url).CurrentValue)
                .WithApiKey(_settingsManager.Nested(x => x.Services.DataReader.ApiKey).CurrentValue)
                .Create();
            builder.RegisterInstance(dataReaderHttpGenerator.Generate<Backend.Contracts.IAccountsApi>())
                .As<Backend.Contracts.IAccountsApi>()
                .SingleInstance();
            builder.RegisterInstance(dataReaderHttpGenerator.Generate<Backend.Contracts.ITradeMonitoringReadingApi>())
                .As<Backend.Contracts.ITradeMonitoringReadingApi>()
                .SingleInstance();
            
            builder.RegisterLykkeServiceClient(_settingsManager.Nested(x => x.Services.ClientAccount.Url).CurrentValue);
            
            builder.Register<IEmailSender>(ctx =>
                new EmailSenderClient(_settingsManager.Nested(x => x.Services.EmailSender.Url).CurrentValue, _log)
            ).SingleInstance();
        }
    }
}
