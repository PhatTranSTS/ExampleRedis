using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly ILogger<VideoController> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly string redisCacheKey = "ViewCount";
        public VideoController(ILogger<VideoController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public string Get()
        {
            var viewCount = new Random().Next().ToString();
            var cacheViewCount = _distributedCache.GetString(redisCacheKey);
            if (string.IsNullOrEmpty(cacheViewCount))
            {
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10));
                _distributedCache.SetString(redisCacheKey, viewCount, options);
                cacheViewCount = _distributedCache.GetString(redisCacheKey);
            }

            return $"Current Views: {viewCount} || Yesterday Views: {cacheViewCount}";
        }
    }
}
