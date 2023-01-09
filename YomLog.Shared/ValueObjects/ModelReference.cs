using YomLog.Shared.Entities;

namespace YomLog.Shared.ValueObjects;

public class ModelReference<T> : ValueObject<ModelReference<T>>
    where T : EntityBase<T>
{
    public long PK { get; protected init; }
    public Guid Id { get; protected init; }

    public ModelReference(T model)
    {
        PK = model.PK;
        Id = model.Id;
    }

    public ModelReference(long pk, Guid id)
    {
        PK = pk;
        Id = id;
    }

    protected override bool EqualsCore(ModelReference<T> other) => Id == other.Id;
}
