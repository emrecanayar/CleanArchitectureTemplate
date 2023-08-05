using Microsoft.EntityFrameworkCore;

namespace Core.Application.Base
{
    public class StringIncludeSpecification<TEntity> where TEntity : class
    {
        public List<string> Includes { get; } = new List<string>();

        public StringIncludeSpecification<TEntity> Include(string includeProperty)
        {
            Includes.Add(includeProperty);
            return this;
        }

        public IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query)
        {
            foreach (var includeProperty in Includes)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
    }
}
