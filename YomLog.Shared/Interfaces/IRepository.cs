using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.Interfaces;

public interface IRepository<TEntity>
    where TEntity : EntityBase<TEntity>
{
    Task CreateAsync(TEntity item);
    Task CreateRangeAsync(IEnumerable<TEntity> items);
    Task UpdateAsync(TEntity item);
    Task UpdateRangeAsync(IEnumerable<TEntity> items);
    Task<TEntity?> FindAsync(Guid id, User? operatedBy = null);
    Task<EntityReference<TEntity>> DeleteAsync(Guid id, User operatedBy);
    Task<List<TEntity>> FindAllAsync(User? operatedBy = null);
    Task<List<TEntity>> FindAllAsync(IEnumerable<Guid> ids, User? operatedBy = null);
    Task<bool> AnyAsync(Guid id);
    Task<bool> AnyAsync(IEnumerable<Guid> ids);
}
