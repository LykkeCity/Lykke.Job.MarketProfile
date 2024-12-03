using Lykke.Job.MarketProfile.Contract;

namespace Lykke.Service.MarketProfile.Models.MarketProfile
{
    public static class AssetPairModelConvertions
    {
        public static AssetPairModel ToApiModel(this AssetPairPrice pair)
        {
            return new AssetPairModel
            {
                AssetPair = pair.AssetPair,
                BidPrice = pair.BidPrice,
                AskPrice = pair.AskPrice,
                BidPriceTimestamp = pair.BidPriceTimestamp,
                AskPriceTimestamp = pair.AskPriceTimestamp
            };
        }
    }
}
