using YomLog.Shared.Entities;
using YomLog.Shared.Queries;

namespace YomLog.Shared.Interfaces;

public interface IFilterQueryService<TEntity, TQueryParameter>
    where TEntity : EntityBase<TEntity>
    where TQueryParameter : IQueryParameter<TEntity>
{
    Task<List<TEntity>> GetPaginatedListAsync(TQueryParameter param, User user);
    Task<List<TEntity>> GetListAsync(TQueryParameter param, User user);
    Task<int> GetCountAsync(TQueryParameter param, User user);
}
