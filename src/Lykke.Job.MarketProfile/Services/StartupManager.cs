using System.Threading.Tasks;
using Antares.Sdk.Services;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.MarketProfile.Domain.Repositories;
using Lykke.Job.MarketProfile.Domain.Services;

namespace Lykke.Job.MarketProfile.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly IAssetPairsRepository _repository;
        private readonly IAssetPairsCacheService _cacheService;
        private readonly ILog _log;

        public StartupManager(
            IAssetPairsRepository repository,
            IAssetPairsCacheService cacheService,
            ILogFactory logFactory)
        {
            _repository = repository;
            _cacheService = cacheService;
            _log = logFactory.CreateLog(this);
        }

        public async Task StartAsync()
        {
            _log.Info("Init cache...");
            await UpdateCacheAsync();
            _log.Info("Init cache finished.");
        }
        
        private async Task UpdateCacheAsync()
        {
            var pairs = await _repository.ReadAsync();
            await _cacheService.InitCacheAsync(pairs);
        }
    }
}
