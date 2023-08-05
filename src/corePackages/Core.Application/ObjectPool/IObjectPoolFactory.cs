using Microsoft.Extensions.ObjectPool;

namespace Core.Application.ObjectPool
{
    public interface IObjectPoolFactory
    {
        ObjectPool<T> Create<T>() where T : class, new();
    }
}
