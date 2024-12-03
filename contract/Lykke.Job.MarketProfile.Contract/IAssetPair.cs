using System;

namespace Lykke.Job.MarketProfile.Contract
{
    public interface IAssetPair
    {
        string AssetPair { get; }
        double BidPrice { get; }
        double AskPrice { get; }
        DateTime BidPriceTimestamp { get; }
        DateTime AskPriceTimestamp { get; }
    }
}
