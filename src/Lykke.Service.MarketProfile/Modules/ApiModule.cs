using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Job.MarketProfile.DomainServices;
using Lykke.Service.MarketProfile.Settings;
using Lykke.SettingsReader;
using StackExchange.Redis;

namespace Lykke.Service.MarketProfile.Modules
{
    [UsedImplicitly]
    public class ApiModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ApiModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_appSettings.CurrentValue.MarketProfileService.Redis.Configuration)); 
                    return lazy.Value;
                })
                .As<IConnectionMultiplexer>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IConnectionMultiplexer>().GetDatabase())
                .As<IDatabase>();
            
            builder.RegisterType<RedisService>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
