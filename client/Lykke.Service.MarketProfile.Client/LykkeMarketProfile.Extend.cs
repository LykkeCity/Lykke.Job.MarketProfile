using System;
using System.Net.Http;

// ReSharper disable once CheckNamespace
namespace Antares.Service.MarketProfile.LykkeClient
{
    public partial class LykkeMarketProfile
    {
        /// <inheritdoc />
        /// <summary>
        /// Should be used to prevent memory leak in RetryPolicy
        /// </summary>
        public LykkeMarketProfile(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();

            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}
