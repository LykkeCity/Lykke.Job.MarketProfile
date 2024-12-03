using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.MarketProfile.Contract;
using Lykke.Job.MarketProfile.Domain.Services;
using Lykke.Job.MarketProfile.NoSql.Models;
using Lykke.Job.QuotesProducer.Contract;

namespace Lykke.Job.MarketProfile.DomainServices
{
    public class AssetPairsCacheService : IAssetPairsCacheService
    {
        private readonly RedisService _redisService;
        private readonly IMyNoSqlWriterWrapper<AssetPairPriceNoSql> _myNoSqlWriterWrapper;
        private ConcurrentDictionary<string, AssetPairPrice> _pairs = new ConcurrentDictionary<string, AssetPairPrice>();

        public AssetPairsCacheService(
            RedisService redisService,
            IMyNoSqlWriterWrapper<AssetPairPriceNoSql> myNoSqlWriterWrapper)
        {
            _redisService = redisService;
            _myNoSqlWriterWrapper = myNoSqlWriterWrapper;
        }

        public Task InitCacheAsync(AssetPairPrice[] pairsToCache)
        {
            var entries = pairsToCache.Select(p => new KeyValuePair<string, AssetPairPrice>(p.AssetPair, p));

            _pairs = new ConcurrentDictionary<string, AssetPairPrice>(entries);

            var tasks = new List<Task>();

            foreach (var pair in _pairs.Values)
            {
                tasks.Add(_redisService.AddAssetPairPriceAsync(pair));
                tasks.Add(_myNoSqlWriterWrapper.TryInsertOrReplaceAsync(AssetPairPriceNoSql.Create(pair)));
            }

            return Task.WhenAll(tasks);
        }

        public Task UpdatePairAsync(QuoteMessage quote)
        {
            var assetPair = _pairs.AddOrUpdate(
                key: quote.AssetPair,
                addValueFactory: assetPairId => Create(quote),
                updateValueFactory: (assetPairId, pair) => UpdateAssetPairPrice(pair, quote));

            return Task.WhenAll(_myNoSqlWriterWrapper.TryInsertOrReplaceAsync(AssetPairPriceNoSql.Create(assetPair)),
                _redisService.AddAssetPairPriceAsync(assetPair));
        }

        public AssetPairPrice[] GetAll()
        {
            return _pairs.Values.ToArray();
        }
        
        private AssetPairPrice Create(QuoteMessage quote)
        {
            var pair = new AssetPairPrice
            {
                AssetPair = quote.AssetPair,
                AskPriceTimestamp = DateTime.MinValue,
                BidPriceTimestamp = DateTime.MinValue
            };

            UpdateAssetPairPrice(pair, quote);

            return pair;
        }

        private AssetPairPrice UpdateAssetPairPrice(AssetPairPrice pair, QuoteMessage quote)
        {
            if (quote.IsBuy)
            {
                if (pair.BidPriceTimestamp < quote.Timestamp)
                {
                    pair.BidPrice = quote.Price;
                    pair.BidPriceTimestamp = quote.Timestamp;
                }
            }
            else
            {
                if (pair.AskPriceTimestamp < quote.Timestamp)
                {
                    pair.AskPrice = quote.Price;
                    pair.AskPriceTimestamp = quote.Timestamp;
                }
            }

            return pair;
        }
    }
}
