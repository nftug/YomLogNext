using YomLog.Shared.Entities;

namespace YomLog.Shared.DTOs;

public interface ICommandDTO<TEntity> where TEntity : EntityBase<TEntity>
{
}