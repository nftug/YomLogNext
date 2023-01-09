using YomLog.Shared.Entities;
using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public abstract class ModelDTOBase<TModel, TModelDTO> : ValueObject<TModelDTO>, IDataTransferObject
    where TModel : EntityBase<TModel>
    where TModelDTO : ModelDTOBase<TModel, TModelDTO>
{
    public Guid Id { get; init; }

    protected ModelDTOBase() { }

    protected ModelDTOBase(EntityBase<TModel> model)
    {
        Id = model.Id;
    }

    protected override bool EqualsCore(TModelDTO other) => Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public abstract override string ToString();
}
