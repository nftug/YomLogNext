using YomLog.Shared.Entities;

namespace YomLog.Shared.Interfaces;

public interface IRepository<TEntity>
    where TEntity : EntityBase<TEntity>
{
    Task CreateAsync(TEntity item);
    Task CreateRangeAsync(IEnumerable<TEntity> items);
    Task UpdateAsync(TEntity item);
    Task<TEntity?> FindAsync(Guid id, User? operatedBy = null);
    Task DeleteAsync(Guid id, User operatedBy);
    Task<List<TEntity>> FindAllAsync(User? operatedBy = null);
    Task<List<TEntity>> FindAllAsync(IEnumerable<Guid> ids, User? operatedBy = null);
}
