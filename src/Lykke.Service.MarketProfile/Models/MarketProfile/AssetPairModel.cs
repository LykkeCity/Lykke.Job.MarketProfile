using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.MarketProfile.Models.MarketProfile
{
    public class AssetPairModel
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