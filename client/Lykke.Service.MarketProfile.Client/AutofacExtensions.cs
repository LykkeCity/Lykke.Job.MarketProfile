using System;
using Autofac;
using JetBrains.Annotations;

namespace Antares.Service.MarketProfile.LykkeClient
{
    [PublicAPI]
    public static class AutofacExtensions
    {
        public static void RegisterMarketProfileClient(this ContainerBuilder builder, string url)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            builder.RegisterInstance<ILykkeMarketProfile>(new LykkeMarketProfile(new Uri(url), new System.Net.Http.HttpClient()));
        }
    }
}
