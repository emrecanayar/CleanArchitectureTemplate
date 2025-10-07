using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Core.Domain.Entities.Base;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TEntityId, TContext> : IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
  where TEntity : Entity<TEntityId>
  where TEntityId : struct, IEquatable<TEntityId>
  where TContext : DbContext
{
    private readonly TContext _context;

    public EfRepositoryBase(TContext context)
    {
        _context = context;
    }

    public IQueryable<TEntity> Query() => _context.Set<TEntity>();

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        entity.ModifiedDate = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
        {
            entity.ModifiedDate = DateTime.UtcNow;
        }

        _context.UpdateRange(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, permanent);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, permanent);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task<IPaginate<TEntity>> GetPagedListAsync(
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      int index = 0,
      int size = 10,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        if (orderBy != null)
        {
            return await orderBy(queryable).ToPaginateAsync(index, size, from: 0, cancellationToken);
        }

        return await queryable.ToPaginateAsync(index, size, from: 0, cancellationToken);
    }

    public async Task<IList<TEntity>> GetToListAsync(
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        if (orderBy != null)
        {
            return await orderBy(queryable).ToListAsync(cancellationToken);
        }

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(
      Expression<Func<TEntity, bool>> predicate,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetPagedListByDynamicAsync(
      DynamicQuery dynamic,
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      int index = 0,
      int size = 10,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.ToPaginateAsync(index, size, from: 0, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetPagedListByDynamicPredicateAsync(DynamicQuery dynamic, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Expression<Func<TEntity, bool>>? predicate = null, int index = 0, int size = 10, bool enableTracking = true, CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query().AsQueryable().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetPagedListByDynamicOrderByAsync(DynamicQuery dynamic, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool enableTracking = true, CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query().AsQueryable().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (orderBy != null)
        {
            return await orderBy(queryable).ToPaginateAsync(index, size, 0, cancellationToken);
        }

        return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetPagedListIgnoreByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool enableTracking = true, CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query().AsQueryable().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.IgnoreQueryFilters().ToPaginateAsync(index, size, 0, cancellationToken);
    }

    public async Task<IList<TEntity>> GetToListByDynamicAsync(
      DynamicQuery dynamic,
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
      Expression<Func<TEntity, bool>>? predicate = null,
      bool withDeleted = false,
      bool enableTracking = true,
      CancellationToken cancellationToken =
      default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.AnyAsync(cancellationToken);
    }

    protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            await setEntityAsSoftDeletedAsync(entity);
        }
        else
        {
            _context.Remove(entity);
        }
    }

    protected async Task SetEntityAsDeletedAsync(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
        {
            await SetEntityAsDeletedAsync(entity, permanent);
        }
    }

    private async Task setEntityAsSoftDeletedAsync(IHasTimestamps entity)
    {
        if (entity.DeletedDate.HasValue)
        {
            return;
        }

        entity.DeletedDate = DateTime.UtcNow;
        entity.IsDeleted = true;

        var navigations = _context
          .Entry(entity)
          .Metadata.GetNavigations()
          .Where(x => x is
          {
              IsOnDependent: false,
              ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade
          })
          .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
            {
                continue;
            }

            if (navigation.PropertyInfo == null)
            {
                continue;
            }

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
                    if (navValue == null)
                    {
                        continue;
                    }
                }

                foreach (IHasTimestamps navValueItem in (IEnumerable)navValue)
                {
                    await setEntityAsSoftDeletedAsync(navValueItem);
                }
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                      .FirstOrDefaultAsync();
                    if (navValue == null)
                    {
                        continue;
                    }
                }

                await setEntityAsSoftDeletedAsync((IHasTimestamps)navValue);
            }
        }

        _context.Update(entity);
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _context.Database.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        _context.Database.RollbackTransaction();
    }

    public TEntity Add(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        _context.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
        {
            entity.CreatedDate = DateTime.UtcNow;
        }

        _context.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public TEntity Update(TEntity entity)
    {
        entity.ModifiedDate = DateTime.UtcNow;
        _context.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        foreach (TEntity entity in entities)
        {
            entity.ModifiedDate = DateTime.UtcNow;
        }

        _context.UpdateRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        SetEntityAsDeleted(entity, permanent);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        SetEntityAsDeleted(entities, permanent);
        _context.SaveChanges();
        return entities;
    }

    public TEntity? Get(
      Expression<Func<TEntity, bool>> predicate,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        return queryable.FirstOrDefault(predicate);
    }

    public IPaginate<TEntity> GetPagedList(
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      int index = 0,
      int size = 10,
      bool withDeleted = false,
      bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        if (orderBy != null)
        {
            return orderBy(queryable).ToPaginate(index, size);
        }

        return queryable.ToPaginate(index, size);
    }

    public IList<TEntity> GetToList(
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        if (orderBy != null)
        {
            return orderBy(queryable).ToList();
        }

        return queryable.ToList();
    }

    public IPaginate<TEntity> GetPagedListByDynamic(
      DynamicQuery dynamic,
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      int index = 0,
      int size = 10,
      bool withDeleted = false,
      bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return queryable.ToPaginate(index, size);
    }

    public IList<TEntity> GetToListByDynamic(
      DynamicQuery dynamic,
      Expression<Func<TEntity, bool>>? predicate = null,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
      bool withDeleted = false,
      bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (include != null)
        {
            queryable = include(queryable);
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return queryable.ToList();
    }

    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
        {
            queryable = queryable.AsNoTracking();
        }

        if (withDeleted)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        return queryable.Any();
    }

    protected void SetEntityAsDeleted(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            setEntityAsSoftDeleted(entity);
        }
        else
        {
            _context.Remove(entity);
        }
    }

    protected void SetEntityAsDeleted(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
        {
            SetEntityAsDeleted(entity, permanent);
        }
    }

    private void setEntityAsSoftDeleted(IHasTimestamps entity)
    {
        if (entity.DeletedDate.HasValue)
        {
            return;
        }

        entity.DeletedDate = DateTime.UtcNow;
        entity.IsDeleted = true;

        var navigations = _context
          .Entry(entity)
          .Metadata.GetNavigations()
          .Where(x => x is
          {
              IsOnDependent: false,
              ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade
          })
          .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
            {
                continue;
            }

            if (navigation.PropertyInfo == null)
            {
                continue;
            }

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToList();
                    if (navValue == null)
                    {
                        continue;
                    }
                }

                foreach (IHasTimestamps navValueItem in (IEnumerable)navValue)
                {
                    setEntityAsSoftDeleted(navValueItem);
                }
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = _context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).FirstOrDefault();
                    if (navValue == null)
                    {
                        continue;
                    }
                }

                setEntityAsSoftDeleted((IHasTimestamps)navValue);
            }
        }

        _context.Update(entity);
    }

    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
          queryProviderType
          .GetMethods()
          .First(m => m is
          {
              Name: nameof(query.Provider.CreateQuery),
              IsGenericMethod: true
          })
          ?.MakeGenericMethod(navigationPropertyType) ??
          throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
          (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression, })!;
        return queryProviderQuery.Where(x => !((IHasTimestamps)x).DeletedDate.HasValue);
    }

    protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        bool hasEntityHaveOneToOneRelation =
          _context
          .Entry(entity)
          .Metadata.GetForeignKeys()
          .All(
            x =>
            x.DependentToPrincipal?.IsCollection == true ||
            x.PrincipalToDependent?.IsCollection == true ||
            x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()) == false;
        if (hasEntityHaveOneToOneRelation)
        {
            throw new InvalidOperationException(
              "Entity has one-to-one relationship. Soft Delete causes problems if you try to create entry again by same foreign key.");
        }
    }
}
