using System.Linq.Expressions;
using System.Reflection;
using Core.Helpers.Enums;
using Microsoft.EntityFrameworkCore;

namespace Core.Helpers.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }

        public static IOrderedQueryable<TSource>? AscOrDescOrder<TSource>(this IQueryable<TSource> query, ESort eSort, string propertyName)
        {
            var entityType = typeof(TSource);

            var propertyInfo = entityType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                ?? entityType.GetProperty("Id");

            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"Unable to find the property '{propertyName}' or 'Id' on type '{entityType.Name}'.");
            }

            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyInfo.Name);
            var selector = Expression.Lambda(property, arg);

            var enumerableType = typeof(Queryable);

            var sortType = eSort == ESort.ASC ? "OrderBy" : "OrderByDescending";

            var method = enumerableType.GetMethods()
                .Single(m => m.Name == sortType && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);

            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            var newQuery = (IOrderedQueryable<TSource>?)genericMethod.Invoke(null, new object[] { query, selector })
                ?? throw new InvalidOperationException("Unable to invoke the generic method.");

            return newQuery;
        }
    }
}
