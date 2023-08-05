namespace Core.Application.Pipelines.Caching.InMemoryCache
{
    public interface ICachableRequestInMemory
    {
        bool BypassCache { get; }
        string CacheKey { get; }
        TimeSpan? SlidingExpiration { get; }
    }
}
