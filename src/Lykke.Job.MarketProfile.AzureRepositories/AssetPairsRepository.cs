using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Job.MarketProfile.Contract;
using Lykke.Job.MarketProfile.Domain.Repositories;
using Newtonsoft.Json;
using AssetPairPrice = Lykke.Job.MarketProfile.Domain.Repositories.AssetPairPrice;

namespace Lykke.Job.MarketProfile.AzureRepositories
{
    public class AssetPairsRepository : IAssetPairsRepository
    {
        private readonly string _container;
        private readonly string _key;
        private readonly IBlobStorage _storage;

        public AssetPairsRepository(IBlobStorage storage, string container, string key)
        {
            _container = container;
            _key = key;
            _storage = storage;
        }

        public async Task<Contract.AssetPairPrice[]> ReadAsync()
        {
            if (!await _storage.HasBlobAsync(_container, _key)) 
                return Array.Empty<Contract.AssetPairPrice>();
            
            var data = await _storage.GetAsync(_container, _key);
            var content = Encoding.UTF8.GetString(data.ToBytes());

            return JsonConvert.DeserializeObject<AssetPairPrice[]>(content)
                .Select(x => x.ToAssetPairPrice())
                .ToArray();
        }

        public Task WriteAsync(AssetPairPrice[] pairs)
        {
            var data = JsonConvert.SerializeObject(pairs).ToUtf8Bytes();

            return _storage.SaveBlobAsync(_container, _key, data);
        }
    }
}
