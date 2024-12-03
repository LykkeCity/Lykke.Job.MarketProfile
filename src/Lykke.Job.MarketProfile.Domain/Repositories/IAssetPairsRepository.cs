using System.Threading.Tasks;
using Lykke.Job.MarketProfile.Contract;

namespace Lykke.Job.MarketProfile.Domain.Repositories
{
    public interface IAssetPairsRepository
    {
        Task<Contract.AssetPairPrice[]> ReadAsync();
        Task WriteAsync(AssetPairPrice[] pairs);
    }
}
