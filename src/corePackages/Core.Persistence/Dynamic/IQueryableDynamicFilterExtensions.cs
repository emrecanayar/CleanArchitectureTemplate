using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Dynamic;

public static class IQueryableDynamicFilterExtensions
{
    private static readonly string[] _orders = { "asc", "desc" };
    private static readonly string[] _logics = { "and", "or" };

    private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
    {
        { "eq", "=" },
        { "neq", "!=" },
        { "lt", "<" },
        { "lte", "<=" },
        { "gt", ">" },
        { "gte", ">=" },
        { "isnull", "== null" },
        { "isnotnull", "!= null" },
        { "startswith", "StartsWith" },
        { "endswith", "EndsWith" },
        { "contains", "Contains" },
        { "doesnotcontain", "Contains" },
        { "any", "any" },
    };

    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
    {
        if (dynamicQuery.Filter is not null)
        {
            query = Filter(query, dynamicQuery.Filter);
        }

        if (dynamicQuery.Sort is not null && dynamicQuery.Sort.Any())
        {
            query = Sort(query, dynamicQuery.Sort);
        }

        return query;
    }

    public static IList<Filter> GetAllFilters(Filter filter)
    {
        List<Filter> filters = new();
        GetFilters(filter, filters);
        return filters;
    }

    public static IQueryable<T> CustomInclude<T>(this IQueryable<T> query, LambdaExpression includeExpression)
    {
        var method = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(m => m.Name == nameof(EntityFrameworkQueryableExtensions.Include) && m.GetParameters().Length == 2);

        if (method == null)
        {
            throw new InvalidOperationException("Include method not found.");
        }

        var genericMethod = method.MakeGenericMethod(typeof(T), includeExpression.ReturnType);

        return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, includeExpression })! ?? throw new InvalidOperationException("Method invocation returned null.");
    }

    public static string Transform(Filter filter, IList<Filter> filters)
    {
        if (string.IsNullOrEmpty(filter.Field))
        {
            throw new ArgumentException("Invalid Field");
        }

        if (string.IsNullOrEmpty(filter.Operator) || !_operators.ContainsKey(filter.Operator))
        {
            throw new ArgumentException("Invalid Operator");
        }

        int index = filters.IndexOf(filter);
        string comparison = _operators[filter.Operator];
        StringBuilder where = new();

        if (!string.IsNullOrEmpty(filter.Value))
        {
            if (filter.Operator == "doesnotcontain")
            {
                where.Append($"(!np({filter.Field}).{comparison}(@{index}))");
            }
            else if (comparison is "StartsWith" or "EndsWith" or "Contains")
            {
                where.Append($"(np({filter.Field}).{comparison}(@{index}))");
            }
            else
            {
                where.Append($"np({filter.Field}) {comparison} @{index}");
            }
        }
        else if (filter.Operator is "isnull" or "isnotnull")
        {
            where.Append($"np({filter.Field}) {comparison}");
        }
        else if (filter.Operator == "any")
        {
            if (filter.Filters != null && filter.Filters.Any())
            {
                var innerFilter = filter.Filters.First();
                int innerIndex = filters.IndexOf(innerFilter);

                if (innerFilter.Operator == "contains")
                {
                    where.Append($"{filter.Field}.Any(x => x.{innerFilter.Field}.{_operators[innerFilter.Operator]}(@{innerIndex}))");
                }
                else
                {
                    string innerComparison = _operators[innerFilter.Operator];
                    where.Append($"{filter.Field}.Any(x => x.{innerFilter.Field} {innerComparison} @{innerIndex})");
                }
            }
        }

        if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
        {
            if (!_logics.Contains(filter.Logic))
            {
                throw new ArgumentException("Invalid Logic");
            }

            return $"{where} {filter.Logic} ({string.Join(separator: $" {filter.Logic} ", value: filter.Filters.Select(f => Transform(f, filters)).ToArray())})";
        }

        return where.ToString();
    }

    private static void GetFilters(Filter filter, IList<Filter> filters)
    {
        filters.Add(filter);
        if (filter.Filters is not null && filter.Filters.Any())
        {
            foreach (Filter item in filter.Filters)
            {
                GetFilters(item, filters);
            }
        }
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        IList<Filter> filters = GetAllFilters(filter);
        string?[] values = filters.Select(f => f.Value).ToArray();
        string where = Transform(filter, filters);
        if (!string.IsNullOrEmpty(where))
        {
            queryable = queryable.Where(where, values);
        }

        return queryable;
    }

    private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        foreach (Sort item in sort)
        {
            if (string.IsNullOrEmpty(item.Field))
            {
                throw new ArgumentException("Invalid Field");
            }

            if (string.IsNullOrEmpty(item.Dir) || !_orders.Contains(item.Dir))
            {
                throw new ArgumentException("Invalid Order Type");
            }
        }

        if (sort.Any())
        {
            string ordering = string.Join(separator: ",", values: sort.Select(s => $"{s.Field} {s.Dir}"));
            return queryable.OrderBy(ordering);
        }

        return queryable;
    }
}
