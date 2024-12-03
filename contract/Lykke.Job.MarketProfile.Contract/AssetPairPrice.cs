using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Job.MarketProfile.Contract
{
    public class AssetPairPrice : IAssetPair
    {
        [Required]
        public string AssetPair { get; set; }
        [Required]
        public double BidPrice { get; set; }
        [Required]
        public double AskPrice { get; set; }
        [Required]
        public DateTime BidPriceTimestamp { get; set; }
        [Required]
        public DateTime AskPriceTimestamp { get; set; }
    }
}
