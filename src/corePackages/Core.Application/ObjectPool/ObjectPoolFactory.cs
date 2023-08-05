using Microsoft.Extensions.ObjectPool;

namespace Core.Application.ObjectPool
{
    public class ObjectPoolFactory : IObjectPoolFactory
    {
        private readonly DefaultObjectPoolProvider _poolProvider;

        public ObjectPoolFactory()
        {
            _poolProvider = new DefaultObjectPoolProvider();
        }

        public ObjectPool<T> Create<T>() where T : class, new()
        {
            return _poolProvider.Create(new DefaultPooledObjectPolicy<T>());
        }
    }
}
