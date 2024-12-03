using System;
using Lykke.Job.MarketProfile.Contract;

namespace Lykke.Job.MarketProfile.Domain.Repositories
{
    public class AssetPairPrice
    {
        public string Code { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public DateTime BidPriceTimestamp { get; set; }
        public DateTime AskPriceTimestamp { get; set; }

        public Contract.AssetPairPrice ToAssetPairPrice()
        {
            return new Contract.AssetPairPrice
            {
                AssetPair = Code,
                BidPrice = BidPrice,
                BidPriceTimestamp = BidPriceTimestamp,
                AskPrice = AskPrice,
                AskPriceTimestamp = AskPriceTimestamp
            };
        }
        
        public static AssetPairPrice Create(Contract.AssetPairPrice pair)
        {
            return new AssetPairPrice
            {
                Code = pair.AssetPair,
                BidPrice = pair.BidPrice,
                BidPriceTimestamp = pair.BidPriceTimestamp,
                AskPrice = pair.AskPrice,
                AskPriceTimestamp = pair.AskPriceTimestamp
            };
        }
    }
}
