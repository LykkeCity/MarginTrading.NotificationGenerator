﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.HttpClientGenerator;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.EmailSender;
using Lykke.SettingsReader;
using MarginTrading.NotificationGenerator.AzureRepositories;
using MarginTrading.NotificationGenerator.Core.Repositories;
using MarginTrading.NotificationGenerator.Core.Services;
using MarginTrading.NotificationGenerator.Services;
using MarginTrading.NotificationGenerator.Settings.JobSettings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace MarginTrading.NotificationGenerator.Modules
{
    public class JobModule : Module
    {
        private readonly IHostingEnvironment _environment;
        private readonly IReloadingManager<NotificationGeneratorSettings> _settingsReloadingManager;
        private readonly NotificationGeneratorSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<NotificationGeneratorSettings> settings, ILog log, IHostingEnvironment environment)
        {
            _log = log;
            _environment = environment;
            _settings = settings.CurrentValue;
            _settingsReloadingManager = settings;

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
            
            builder.RegisterInstance(_settings).As<NotificationGeneratorSettings>().SingleInstance();

            builder.RegisterType<SystemClock>().As<ISystemClock>().SingleInstance();
            
            builder.RegisterType<EmailService>().As<IEmailService>().SingleInstance();
            
            builder.Register<ITemplateGenerator>(ctx =>
                new MustacheTemplateGenerator(_environment, "EmailTemplates")
            ).SingleInstance();
            
            builder.RegisterType<TradingReportService>()
                .As<ITradingReportService>()
                .WithParameters(new []
                {
                    new NamedParameter("monthlyNotificationsSettings", _settings.MonthlyTradingReportSettings),
                    new NamedParameter("dailyNotificationsSettings", _settings.DailyTradingReportSettings), 
                })
                .SingleInstance();
            
            builder.RegisterType<ConvertService>().As<IConvertService>().SingleInstance();
			
            builder.Register<IOvernightSwapHistoryRepository>(ctx =>
                AzureRepoFactories.MarginTrading.CreateOvernightSwapHistoryRepository(_settingsReloadingManager
                    .Nested(x => x.Db.HistoryConnString), _log)
            ).SingleInstance();

            builder.Populate(_services);
        }
    }
}
