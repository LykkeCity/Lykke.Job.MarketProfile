using Antares.Sdk;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.MarketProfile.Settings;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Swisschain.Sdk.Metrics.Rest;

namespace Lykke.Service.MarketProfile
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "Lykke Market Profile",
            ApiVersion = "v1"
        };

        private LykkeServiceOptions<AppSettings> _lykkeOptions;
        private IReloadingManagerWithConfiguration<AppSettings> _settings;


        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            (_lykkeOptions, _settings) = services.ConfigureServices<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "MarketProfileServiceLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.MarketProfileService.Db.LogsConnectionString;
                };

                options.Extend = (sc, settingsManager) =>
                {
                    sc.AddMemoryCache();
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricServer();

            app.UseMiddleware<PrometheusMetricsMiddleware>();

            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }

        [UsedImplicitly]
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.ConfigureContainerBuilder(_lykkeOptions, configurationRoot, _settings);
        }
    }
}
