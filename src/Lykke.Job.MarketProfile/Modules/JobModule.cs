using System;
using Antares.Sdk.Health;
using Antares.Sdk.Services;
using Autofac;
using AzureStorage.Blob;
using Lykke.Job.MarketProfile.AzureRepositories;
using Lykke.Job.MarketProfile.Domain.Repositories;
using Lykke.Job.MarketProfile.Domain.Services;
using Lykke.Job.MarketProfile.DomainServices;
using Lykke.Job.MarketProfile.PeriodicalHandlers;
using Lykke.Job.MarketProfile.RabbitMqSubscribers;
using Lykke.Job.MarketProfile.Services;
using Lykke.Job.MarketProfile.Settings;
using Lykke.SettingsReader;
using StackExchange.Redis;

namespace Lykke.Job.MarketProfile.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();
            
            builder.RegisterType<PersistHandler>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.MarketProfileJob.PersistPeriod))
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();

            builder.Register<IAssetPairsRepository>(
                x => new AssetPairsRepository(
                    AzureBlobStorage.Create(_settings.ConnectionString(c => c.MarketProfileJob.Db.AssetPairsPricesConnString)),
                    _settings.CurrentValue.MarketProfileJob.Blob.Container,
                    _settings.CurrentValue.MarketProfileJob.Blob.Key));

            builder.RegisterType<AssetPairsCacheService>()
                .As<IAssetPairsCacheService>()
                .SingleInstance();

            builder.RegisterType<QuotesSubscriber>()
                .WithParameter(new NamedParameter("connectionString", _settings.CurrentValue.MarketProfileJob.RabbitMq.QuotesConnectionString))
                .WithParameter(new NamedParameter("exchangeName", _settings.CurrentValue.MarketProfileJob.RabbitMq.QuotesExchangeName))
                .WithParameter(new NamedParameter("queueSuffix", _settings.CurrentValue.MarketProfileJob.RabbitMq.QueueSuffix))
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            
            builder.Register(c =>
                {
                    var lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_settings.CurrentValue.MarketProfileJob.Redis.Configuration)); 
                    return lazy.Value;
                })
                .As<IConnectionMultiplexer>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IConnectionMultiplexer>().GetDatabase())
                .As<IDatabase>();
            
            builder.RegisterType<RedisService>().SingleInstance();
        }
    }
}
