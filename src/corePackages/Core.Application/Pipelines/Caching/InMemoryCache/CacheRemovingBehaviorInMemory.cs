using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.Caching.InMemoryCache
{
    public class CacheRemovingBehaviorInMemory<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheRemoverRequestInMemory
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheRemovingBehaviorInMemory<TRequest, TResponse>> _logger;

        public CacheRemovingBehaviorInMemory(IMemoryCache cache, ILogger<CacheRemovingBehaviorInMemory<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response;
            if (request.BypassCache) return await next();

            async Task<TResponse> GetResponseAndRemoveCache()
            {
                response = await next();
                _cache.Remove(request.CacheKey);
                return response;
            }

            response = await GetResponseAndRemoveCache();
            _logger.LogInformation($"Removed Cache -> {request.CacheKey}");

            return response;
        }
    }
}
