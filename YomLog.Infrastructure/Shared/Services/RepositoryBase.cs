using System.Linq.Expressions;
using LiteDB.Async;
using YomLog.Infrastructure.Shared.DataModels;
using YomLog.Infrastructure.Shared.LiteDB;
using YomLog.Shared.Attributes;
using YomLog.Shared.Entities;
using YomLog.Shared.Exceptions;
using YomLog.Shared.Interfaces;

namespace YomLog.Infrastructure.Shared.Services;

[InjectAsTransient]
public abstract class RepositoryBase<TEntity, TDataModel> : IRepository<TEntity>
    where TEntity : EntityBase<TEntity>
    where TDataModel : DataModelBase<TEntity, TDataModel>, new()
{
    protected readonly AppConfig _appConfig;
    protected LiteDbCollection<TEntity, TDataModel> Context => new(_appConfig);

    protected virtual ILiteQueryableAsync<TDataModel> GetCollectionForQuery(ILiteCollectionAsync<TDataModel> collection)
        => collection.Query();

    protected Task<T> UseCollectionQuery<T>(Func<ILiteQueryableAsync<TDataModel>, Task<T>> callback)
    {
        using var context = Context;
        return callback(GetCollectionForQuery(context.Collection));
    }

    public RepositoryBase(AppConfig appConfig)
    {
        _appConfig = appConfig;
    }

    public virtual async Task CreateAsync(TEntity item)
    {
        using var context = Context;
        var data = new TDataModel().Transfer(item);
        await context.Collection.InsertAsync(data);
    }

    public virtual async Task CreateRangeAsync(IEnumerable<TEntity> items)
    {
        using var context = Context;
        var data = items.Select(x => new TDataModel().Transfer(x));
        await context.Collection.InsertBulkAsync(data);
    }

    public virtual async Task UpdateAsync(TEntity item)
    {
        using var context = Context;
        var data = new TDataModel().Transfer(item);
        await context.Collection.UpdateAsync(data);
    }

    public virtual Task<TEntity?> FindAsync(Guid id, User? operatedBy = null)
        => FindByPredicateAsync(x => x.Id == id, operatedBy);

    public virtual async Task DeleteAsync(Guid id, User operatedBy)
    {
        var item = await FindAsync(id, operatedBy) ?? throw new NotFoundException();
        if (!item.CheckCanDelete(operatedBy)) throw new ForbiddenException();

        using var context = Context;
        await context.Collection.DeleteAsync(item.PK);
    }

    public virtual Task<List<TEntity>> FindAllAsync(User? operatedBy = null)
        => FindAllByPredicateAsync(null, operatedBy);

    public virtual Task<List<TEntity>> FindAllAsync(IEnumerable<Guid> ids, User? operatedBy = null)
        => FindAllByPredicateAsync(x => ids.Contains(x.Id), operatedBy);

    internal Task<TEntity?> FindByPredicateAsync(
        Expression<Func<TDataModel, bool>> predicate,
        User? operatedBy = null
    )
        => UseCollectionQuery(async query =>
        {
            var entity = (await query.Where(predicate).FirstOrDefaultAsync())?.ToDomain();
            if (entity == null) return null;
            if (operatedBy != null && !entity.CheckCanGet(operatedBy))
                throw new ForbiddenException();

            return entity;
        });

    internal Task<List<TEntity>> FindAllByPredicateAsync(
        Expression<Func<TDataModel, bool>>? predicate = null,
        User? operatedBy = null
    )
        => UseCollectionQuery(async query =>
                (await query
                    .Where(predicate ?? (x => true))
                    .ToListAsync())
                    .Select(x => x.ToDomain())
                    .Where(x => operatedBy == null || x.CheckCanGet(operatedBy))
                    .ToList()
            );
}
