namespace Core.Application.Pipelines.Caching.DisturbedCache
{
    public interface ICachableRequest
    {
        bool BypassCache { get; }
        string CacheKey { get; }
        TimeSpan? SlidingExpiration { get; }
    }
}
