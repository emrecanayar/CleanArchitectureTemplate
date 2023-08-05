namespace Core.Application.Pipelines.Caching.InMemoryCache
{
    public interface ICacheRemoverRequestInMemory
    {
        bool BypassCache { get; }
        string CacheKey { get; }
    }
}
