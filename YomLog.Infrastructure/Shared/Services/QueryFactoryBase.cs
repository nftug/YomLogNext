using YomLog.Infrastructure.Shared.DAOs;
using YomLog.Shared.Entities;

namespace YomLog.Infrastructure.Shared.Services;

public abstract class QueryFactoryBase<TEntity, TEntityDAO> : IQueryFactory<TEntity, TEntityDAO>
    where TEntity : EntityBase<TEntity>
    where TEntityDAO : EntityDAOBase<TEntity, TEntityDAO>, new()
{
    protected readonly DataContext _context;

    public QueryFactoryBase(DataContext context)
    {
        _context = context;
    }

    public virtual IQueryable<TEntityDAO> Source => _context.Set<TEntityDAO>();
    public virtual IQueryable<TEntityDAO> ListSource => Source;
}

public interface IQueryFactory<TEntity, TEntityDAO>
    where TEntity : EntityBase<TEntity>
    where TEntityDAO : EntityDAOBase<TEntity, TEntityDAO>, new()
{
    IQueryable<TEntityDAO> Source { get; }
    IQueryable<TEntityDAO> ListSource { get; }
}
