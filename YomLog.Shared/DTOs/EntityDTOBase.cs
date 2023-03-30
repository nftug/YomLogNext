using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public abstract class EntityDTOBase<TEntity, TEntityDTO> : ValueObject<TEntityDTO>, IDataTransferObject
    where TEntity : EntityBase<TEntity>
    where TEntityDTO : EntityDTOBase<TEntity, TEntityDTO>
{
    public Guid Id { get; init; }

    public bool IsNewItem => Id == default;

    protected EntityDTOBase() { }

    protected EntityDTOBase(EntityBase<TEntity> model)
    {
        Id = model.Id;
    }

    protected override bool EqualsCore(TEntityDTO other) => Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public abstract override string ToString();
}
