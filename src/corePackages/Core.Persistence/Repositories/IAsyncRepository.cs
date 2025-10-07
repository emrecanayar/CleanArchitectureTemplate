using System.Linq.Expressions;
using Core.Domain.Entities.Base;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Persistence.Repositories;

public interface IAsyncRepository<TEntity, TEntityId> : IQuery<TEntity>, ITransaction
    where TEntity : Entity<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
{
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetToListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetPagedListByDynamicAsync(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetToListByDynamicAsync(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetPagedListByDynamicPredicateAsync(DynamicQuery dynamic,
                                       Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                       Expression<Func<TEntity, bool>>? predicate = null,
                                       int index = 0, int size = 10, bool enableTracking = true,
                                       CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetPagedListByDynamicOrderByAsync(DynamicQuery dynamic,
                                             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                             Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                             int index = 0, int size = 10, bool enableTracking = true,
                                             CancellationToken cancellationToken = default);

    Task<IPaginate<TEntity>> GetPagedListIgnoreByDynamicAsync(DynamicQuery dynamic,
                                     Expression<Func<TEntity, bool>>? predicate = null,
                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                     int index = 0, int size = 10, bool enableTracking = true,
                                     CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity);

    Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entity);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entity);

    Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false);

    Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entity, bool permanent = false);
}
