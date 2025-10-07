using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Core.Helpers.Extensions
{
    public static class ContextExtensions
    {
        public static DbSet<T>? Set<T>(this DbContext context, Type t)
            where T : class
        {
            MethodInfo? setMethod = context.GetType().GetMethod("Set");
            if (setMethod == null)
            {
                throw new InvalidOperationException("Unable to find the 'Set' method on DbContext.");
            }

            return (DbSet<T>?)setMethod.MakeGenericMethod(t).Invoke(context, null);
        }

        public static IQueryable<T> QueryableOf<T>(this DbContext context, string typeName)
            where T : class
        {
            IEntityType? type = context.Model.FindEntityType(typeName);
            if (type == null)
            {
                throw new InvalidOperationException($"Unable to find the entity type '{typeName}'.");
            }

            MethodInfo? setMethod = context.GetType().GetMethod("Set");
            if (setMethod == null)
            {
                throw new InvalidOperationException("Unable to find the 'Set' method on DbContext.");
            }

            Type clearType = type.ClrType;

            IQueryable? q = (IQueryable?)setMethod.MakeGenericMethod(clearType).Invoke(context, null);
            if (q == null)
            {
                throw new InvalidOperationException("The 'Set' method returned null.");
            }

            return q.OfType<T>();
        }
    }
}
