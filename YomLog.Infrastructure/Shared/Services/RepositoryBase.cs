using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.Shared.DAOs;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;
using YomLog.Shared.Interfaces;
using YomLog.Shared.ValueObjects;

namespace YomLog.Infrastructure.Shared.Services;

public abstract class RepositoryBase<TEntity, TEntityDAO> : IRepository<TEntity>
    where TEntity : EntityBase<TEntity>
    where TEntityDAO : EntityDAOBase<TEntity, TEntityDAO>, new()
{
    protected readonly DataContext _context;
    protected readonly IQueryFactory<TEntity, TEntityDAO> _queryFactory;

    public RepositoryBase(DataContext context, IQueryFactory<TEntity, TEntityDAO> queryFactory)
    {
        _context = context;
        _queryFactory = queryFactory;
    }

    public virtual async Task CreateAsync(TEntity item)
    {
        var data = new TEntityDAO().Transfer(item);
        await _context.AddAsync(data);

        // Apply navigation after saving to database, because the operation requires PK.
        data.ApplyNavigation(item);
        await _context.SaveChangesAsync();
    }

    public virtual async Task CreateRangeAsync(IEnumerable<TEntity> items)
    {
        foreach (var item in items)
        {
            var data = new TEntityDAO().Transfer(item);
            await _context.AddAsync(data);
            data.ApplyNavigation(item);
        }

        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity item)
    {
        var data = await _context.Set<TEntityDAO>().AsTracking()
            .FirstOrDefaultAsync(x => x.Id == item.Id)
            ?? throw new NotFoundException();

        await UpdateCoreAsync(data, item);
        await _context.SaveChangesAsync();
    }

    protected virtual async Task UpdateCoreAsync(TEntityDAO data, TEntity item)
    {
        data.Transfer(item);

        // Clear many-to-many relationships (Physical delete)
        await ClearJoinTables(data);
        // Apply a new navigation.
        data.ApplyNavigation(item);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> items)
    {
        var ids = items.Select(x => x.Id);
        var entities = await _context.Set<TEntityDAO>()
            .AsTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();

        foreach (var entity in entities)
        {
            var item = items.First(x => x.Id == entity.Id);
            await UpdateCoreAsync(entity, item);
        }

        await _context.SaveChangesAsync();
    }

    public virtual Task<TEntity?> FindAsync(Guid id, User? operatedBy = null)
        => FindByPredicateAsync(x => x.Id == id, operatedBy);

    public virtual async Task<EntityReference<TEntity>> DeleteAsync(Guid id, User operatedBy)
    {
        var item = await FindAsync(id, operatedBy);
        if (item == null) throw new NotFoundException();
        if (!item.CheckCanDelete(operatedBy)) throw new ForbiddenException();

        var data = await _context.Set<TEntityDAO>()
            .AsTracking()
            .FirstAsync(x => x.Id == id);

        _context.Remove(data);
        await _context.SaveChangesAsync();

        // returns deleted item's reference
        return new(data.PK, data.Id);
    }

    public virtual Task<List<TEntity>> FindAllAsync(User? operatedBy = null)
        => FindAllByPredicateAsync(null, operatedBy);

    public virtual Task<List<TEntity>> FindAllAsync(IEnumerable<Guid> ids, User? operatedBy = null)
        => FindAllByPredicateAsync(x => ids.Contains(x.Id), operatedBy);

    public virtual Task<bool> AnyAsync(Guid id)
        => _context.Set<TEntityDAO>().AnyAsync(x => x.Id == id);

    public virtual Task<bool> AnyAsync(IEnumerable<Guid> ids)
        => _context.Set<TEntityDAO>().AnyAsync(x => ids.Contains(x.Id));

    internal async Task<TEntity?> FindByPredicateAsync(
        Expression<Func<TEntityDAO, bool>> predicate,
        User? operatedBy = null
    )
    {
        // If operated by admin users, allow them to get any deleted items.
        var source = _queryFactory.Source;
        if (operatedBy?.Role == UserRole.Admin) source = source.IgnoreQueryFilters();

        var item = (await source.FirstOrDefaultAsync(predicate))?.ToDomain();

        if (item == null) return null;
        if (operatedBy != null && !item.CheckCanGet(operatedBy))
            throw new ForbiddenException();

        return item;
    }

    internal async Task<List<TEntity>> FindAllByPredicateAsync(
        Expression<Func<TEntityDAO, bool>>? predicate = null,
        User? operatedBy = null
    )
        => (await _queryFactory.Source
            .Where(predicate ?? (x => true))
            .ToListAsync())
            .Select(x => x.ToDomain())
            .Where(x => operatedBy == null || x.CheckCanGet(operatedBy))
            .ToList();

    /// <summary>
    /// Clear many-to-many relationships with physical deleting. Use before updating an entity.
    /// </summary>
    protected async Task ClearJoinTables(IEntityDAO data)
    {
        var joinTables = _context.Entry(data).Collections
            .Where(x =>
                x.Metadata.PropertyInfo != null &&
                typeof(IJoinTableDAO).IsAssignableFrom(x.Metadata.PropertyInfo.PropertyType.GetGenericArguments()[0])
            );

        foreach (var collectionEntry in joinTables)
        {
            await collectionEntry.LoadAsync();
            _context.RemoveRange((IEnumerable<IJoinTableDAO>)collectionEntry.CurrentValue!);
        }
    }
}
