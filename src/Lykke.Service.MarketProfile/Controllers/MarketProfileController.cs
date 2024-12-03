using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Job.MarketProfile.DomainServices;
using Lykke.Service.MarketProfile.Models;
using Lykke.Service.MarketProfile.Models.MarketProfile;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.MarketProfile.Controllers
{
    [Route("api/[controller]")]
    public class MarketProfileController : Controller
    {
        private readonly RedisService _redisService;

        public MarketProfileController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("")]
        public async Task<IEnumerable<AssetPairModel>> GetAll()
        {
            var pairs = await _redisService.GetMarketProfilesAsync();

            return pairs.Select(p => p.ToApiModel());
        }

        [HttpGet("{pairCode}")]
        [ProducesResponseType(typeof(AssetPairModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Get(string pairCode)
        {
            if (string.IsNullOrWhiteSpace(pairCode))
            {
                return BadRequest(new ErrorModel
                {
                    Code = ErrorCode.InvalidInput,
                    Message = "Pair code is required"
                });
            }

            var pair = await _redisService.GetMarketProfileAsync(pairCode);

            if (pair == null)
            {
                return NotFound(new ErrorModel
                {
                    Code = ErrorCode.PairNotFound,
                    Message  = "Pair not found"
                });
            }

            return Ok(pair.ToApiModel());
        }
    }
}
