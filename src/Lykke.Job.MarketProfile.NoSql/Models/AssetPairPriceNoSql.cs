using System;
using Lykke.Job.MarketProfile.Contract;
using MyNoSqlServer.Abstractions;

namespace Lykke.Job.MarketProfile.NoSql.Models
{
    public class AssetPairPriceNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "antares_market-profile_asset-pair-prices";

        public static string GeneratePartitionKey() => "AssetPairPrice";
        public static string GenerateRowKey(string assetPairId) => assetPairId;

        public AssetPairModel AssetPair { get; set; }

        public static AssetPairPriceNoSql Create(IAssetPair source)
        {
            var item = new AssetPairPriceNoSql() {PartitionKey = GeneratePartitionKey(), RowKey = GenerateRowKey(source.AssetPair), AssetPair = AssetPairModel.Create(source)};
            return item;
        }

        public class AssetPairModel : IAssetPair
        {
            public static AssetPairModel Create(IAssetPair source)
            {
                var item = new AssetPairModel()
                {
                    AskPrice = source.AskPrice,
                    AskPriceTimestamp = source.AskPriceTimestamp,
                    BidPrice = source.BidPrice,
                    BidPriceTimestamp = source.BidPriceTimestamp,
                    AssetPair = source.AssetPair
                };

                return item;
            }

            public string AssetPair { get; set; }
            public double BidPrice { get; set; }
            public double AskPrice { get; set; }
            public DateTime BidPriceTimestamp { get; set; }
            public DateTime AskPriceTimestamp { get; set; }
        }
    }
}
