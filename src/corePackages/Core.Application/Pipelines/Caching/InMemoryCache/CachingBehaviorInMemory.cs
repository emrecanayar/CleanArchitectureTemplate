using Core.Application.Pipelines.Caching.DisturbedCache;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Core.Application.Pipelines.Caching.InMemoryCache
{
    public class CachingBehaviorInMemory<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICachableRequestInMemory
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingBehaviorInMemory<TRequest, TResponse>> _logger;
        private readonly CacheSettings _cacheSettings;

        public CachingBehaviorInMemory(IMemoryCache cache, ILogger<CachingBehaviorInMemory<TRequest, TResponse>> logger, IConfiguration configuration)
        {
            _cache = cache;
            _logger = logger;
            _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response;
            if (request.BypassCache) return await next();

            async Task<TResponse> GetResponseAndAddToCache()
            {
                response = await next();
                TimeSpan? slidingExpiration = request.SlidingExpiration ?? TimeSpan.FromDays(_cacheSettings.SlidingExpiration);
                MemoryCacheEntryOptions cacheOptions = new() { SlidingExpiration = slidingExpiration };
                byte[] serializeData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                _cache.Set(request.CacheKey, serializeData, cacheOptions);
                return response;
            }

            byte[]? cachedResponse = (byte[]?)_cache.Get(request.CacheKey);
            if (cachedResponse != null)
            {
                response = JsonConvert.DeserializeObject<TResponse>(Encoding.Default.GetString(cachedResponse));
                _logger.LogInformation($"Fetched from Cache -> {request.CacheKey}");
            }
            else
            {
                response = await GetResponseAndAddToCache();
                _logger.LogInformation($"Added to Cache -> {request.CacheKey}");
            }

            return response;
        }
    }
}
