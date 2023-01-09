using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.Interfaces;

public interface IRepository<TModel>
    where TModel : EntityBase<TModel>
{
    Task<TModel> CreateAsync(TModel item);
    Task<TModel> UpdateAsync(TModel item);
    Task UpdateRangeAsync(IEnumerable<TModel> items);
    Task<TModel?> FindAsync(Guid id, User? operatedBy = null);
    Task<ModelReference<TModel>> DeleteAsync(Guid id, User operatedBy);
    Task<List<TModel>> FindAllAsync(User? operatedBy = null);
    Task<List<TModel>> FindAllAsync(IEnumerable<Guid> ids, User? operatedBy = null);
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(IEnumerable<Guid> ids);
}
