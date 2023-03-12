using System.Text.Json.Serialization;
using YomLog.Shared.Entities;

namespace YomLog.Shared.ValueObjects;

public class EntityReference<T> : ValueObject<EntityReference<T>>, IEntityReference
    where T : EntityBase<T>
{
    public long PK { get; init; }
    public Guid Id { get; init; }

    public EntityReference(T Entity)
    {
        PK = Entity.PK;
        Id = Entity.Id;
    }

    public EntityReference(long pk, Guid id)
    {
        PK = pk;
        Id = id;
    }

    [JsonConstructor] public EntityReference() { }

    protected override bool EqualsCore(EntityReference<T> other) => Id == other.Id;
    public override string ToString() => Id.ToString();
}

public class EntityReferenceWithName<T> : EntityReference<T>, IEntityReferenceWithName
    where T : EntityWithNameBase<T>
{
    public string Name { get; init; } = string.Empty;

    public EntityReferenceWithName(T Entity) : base(Entity)
    {
        Name = Entity.Name;
    }

    public EntityReferenceWithName(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public EntityReferenceWithName(long pk, Guid id, string name) : this(id, name)
    {
        PK = pk;
    }

    [JsonConstructor] public EntityReferenceWithName() { }

    public override string ToString() => Name;
}

public interface IEntityReference
{
    long PK { get; }
    Guid Id { get; }
}

public interface IEntityReferenceWithName : IEntityReference
{
    string Name { get; }
}