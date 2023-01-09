using YomLog.Shared.Entities;
using YomLog.Shared.Queries;

namespace YomLog.Shared.Interfaces;

public interface IFilterQueryService<TModel, TQueryParameter>
    where TModel : EntityBase<TModel>
    where TQueryParameter : IQueryParameter<TModel>
{
    Task<List<TModel>> GetPaginatedListAsync(TQueryParameter param, User user);
    Task<List<TModel>> GetListAsync(TQueryParameter param, User user);
    Task<int> GetCountAsync(TQueryParameter param, User user);
}
