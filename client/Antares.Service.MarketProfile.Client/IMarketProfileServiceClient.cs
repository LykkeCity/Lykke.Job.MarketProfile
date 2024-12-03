using System.Collections.Generic;
using Antares.Service.MarketProfile.LykkeClient;
using Autofac;
using Lykke.Job.MarketProfile.Contract;

namespace Antares.Service.MarketProfile.Client
{
    public interface IMarketProfileServiceClient
    {
        IAssetPair Get(string id);
        List<IAssetPair> GetAll();

        ILykkeMarketProfile HttpClient { get; }

        void Start();
    }
}
