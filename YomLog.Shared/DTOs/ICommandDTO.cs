using YomLog.Shared.Entities;

namespace YomLog.Shared.DTOs;

public interface ICommandDTO<TModel>
    where TModel : EntityBase<TModel>
{
}