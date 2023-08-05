namespace Core.Application.Pipelines.Caching.DisturbedCache
{
    public interface ICacheRemoverRequest
    {
        bool BypassCache { get; }
        string CacheKey { get; }
    }
}
