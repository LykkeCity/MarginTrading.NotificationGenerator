using Autofac;
using Common.Log;
using Lykke.HttpClientGenerator;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.EmailSender;
using Lykke.SettingsReader;
using MarginTrading.NotificationGenerator.Settings.JobSettings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MarginTrading.NotificationGenerator.Modules
{
    public class ExternalServicesModule : Module
    {
        private readonly IHostingEnvironment _environment;
        private readonly NotificationGeneratorSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ExternalServicesModule(NotificationGeneratorSettings settings, ILog log, 
            IHostingEnvironment environment)
        {
            _log = log;
            _environment = environment;
            _settings = settings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            var dataReaderHttpGenerator = HttpClientGenerator
                .BuildForUrl(_settings.Services.DataReader.Url)
                .WithApiKey(_settings.Services.DataReader.ApiKey)
                .Create();
            builder.RegisterInstance(dataReaderHttpGenerator.Generate<Backend.Contracts.IAccountsApi>())
                .As<Backend.Contracts.IAccountsApi>()
                .SingleInstance();
            builder.RegisterInstance(dataReaderHttpGenerator.Generate<Backend.Contracts.ITradeMonitoringReadingApi>())
                .As<Backend.Contracts.ITradeMonitoringReadingApi>()
                .SingleInstance();
            builder.RegisterInstance(dataReaderHttpGenerator.Generate<Backend.Contracts.IAccountHistoryApi>())
                .As<Backend.Contracts.IAccountHistoryApi>()
                .SingleInstance();
            
            builder.RegisterLykkeServiceClient(_settings.Services.ClientAccount.Url);

            builder.Register<IEmailSender>(ctx => new EmailSenderClient(_settings.Services.EmailSender.Url, _log))
                .SingleInstance();
        }
    }
}