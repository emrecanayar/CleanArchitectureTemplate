using Core.Persistence.Dynamic;
using System.Linq.Expressions;

namespace Core.Application.Base
{
    public class IncludeSpecification<TEntity> where TEntity : class
    {
        public List<LambdaExpression> Includes { get; } = new List<LambdaExpression>();

        public IncludeSpecification<TEntity> Include(LambdaExpression includeExpression)
        {
            Includes.Add(includeExpression);
            return this;
        }

        public IQueryable<TEntity> BuildQuery(IQueryable<TEntity> query)
        {
            return Includes.Aggregate(query, (q, include) => q.CustomInclude(include));
        }
    }
}
