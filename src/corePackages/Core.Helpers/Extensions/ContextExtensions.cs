using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Core.Helpers.Extensions
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Finds the Set of the given type from within the given Db context and returns a query object cast to the requested type.
        /// The given T type must be implemented by the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static DbSet<T>? Set<T>(this DbContext context, Type t) where T : class
        {
            MethodInfo? setMethod = context.GetType().GetMethod("Set");
            if (setMethod == null)
            {
                throw new InvalidOperationException("Unable to find the 'Set' method on DbContext.");
            }

            return (DbSet<T>?)setMethod.MakeGenericMethod(t).Invoke(context, null);
        }


        /// <summary>
        /// Returns the DbSet object as a queryable of the desired type (T).
        ///
        /// Here the object attached to DbContext should implement the T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static IQueryable<T> QueryableOf<T>(this DbContext context, string typeName) where T : class
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