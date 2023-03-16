using YomLog.Infrastructure.Shared.EDMs;
using YomLog.Shared.Attributes;
using YomLog.Shared.Entities;

namespace YomLog.Infrastructure.Shared.Services;

[InjectAsTransient]
public abstract class QueryFactoryBase<TEntity, TEntityEDM> : IQueryFactory<TEntity, TEntityEDM>
    where TEntity : EntityBase<TEntity>
    where TEntityEDM : EntityEDMBase<TEntity, TEntityEDM>, new()
{
    protected readonly DataContext _context;

    public QueryFactoryBase(DataContext context)
    {
        _context = context;
    }

    public virtual IQueryable<TEntityEDM> Source => _context.Set<TEntityEDM>();
    public virtual IQueryable<TEntityEDM> ListSource => Source;
}

public interface IQueryFactory<TEntity, TEntityEDM>
    where TEntity : EntityBase<TEntity>
    where TEntityEDM : EntityEDMBase<TEntity, TEntityEDM>, new()
{
    IQueryable<TEntityEDM> Source { get; }
    IQueryable<TEntityEDM> ListSource { get; }
}
