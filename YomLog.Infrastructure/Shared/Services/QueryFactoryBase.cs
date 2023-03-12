using YomLog.Infrastructure.Shared.DataModels;
using YomLog.Shared.Entities;

namespace YomLog.Infrastructure.Shared.Services;

public abstract class QueryFactoryBase<TEntity, TDataModel> : IQueryFactory<TEntity, TDataModel>
    where TEntity : EntityBase<TEntity>
    where TDataModel : DataModelBase<TEntity, TDataModel>, new()
{
    protected readonly DataContext _context;

    public QueryFactoryBase(DataContext context)
    {
        _context = context;
    }

    public virtual IQueryable<TDataModel> Source => _context.Set<TDataModel>();
    public virtual IQueryable<TDataModel> ListSource => Source;
}

public interface IQueryFactory<TEntity, TDataModel>
    where TEntity : EntityBase<TEntity>
    where TDataModel : DataModelBase<TEntity, TDataModel>, new()
{
    IQueryable<TDataModel> Source { get; }
    IQueryable<TDataModel> ListSource { get; }
}
