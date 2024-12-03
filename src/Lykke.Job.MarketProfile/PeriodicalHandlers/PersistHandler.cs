using System;
using System.Linq;
using System.Threading;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.MarketProfile.Domain.Repositories;
using Lykke.Job.MarketProfile.Domain.Services;
using Lykke.Job.MarketProfile.DomainServices;
using Lykke.Job.MarketProfile.NoSql.Models;

namespace Lykke.Job.MarketProfile.PeriodicalHandlers
{
    [UsedImplicitly]
    public class PersistHandler: IStartable, IDisposable
    {
        private readonly TimeSpan _persistPeriod;
        private readonly IMyNoSqlWriterWrapper<AssetPairPriceNoSql> _myNoSqlWriterWrapper;
        private readonly IAssetPairsCacheService _cacheService;
        private readonly IAssetPairsRepository _repository;
        private Timer _timer;
        private readonly ILog _log;

        public PersistHandler(
            TimeSpan persistPeriod,
            IMyNoSqlWriterWrapper<AssetPairPriceNoSql> myNoSqlWriterWrapper,
            IAssetPairsCacheService cacheService,
            IAssetPairsRepository repository,
            ILogFactory logFactory
            )
        {
            _persistPeriod = persistPeriod;
            _myNoSqlWriterWrapper = myNoSqlWriterWrapper;
            _cacheService = cacheService;
            _repository = repository;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            _timer = new Timer(PersistCache, null, _persistPeriod, Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        
        private async void PersistCache(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                var pairs = _cacheService.GetAll();

                await _repository.WriteAsync(pairs.Select(AssetPairPrice.Create).ToArray());
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                _timer.Change(_persistPeriod, Timeout.InfiniteTimeSpan);
            }
        }
    }
}
