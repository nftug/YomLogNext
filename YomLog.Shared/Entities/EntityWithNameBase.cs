namespace YomLog.Shared.Entities;

public abstract class EntityWithNameBase<T> : EntityBase<T>
    where T : EntityWithNameBase<T>
{
    public string Name { get; protected set; } = string.Empty;

    protected EntityWithNameBase() { }

    protected EntityWithNameBase(string name)
    {
        Name = name;
    }
}
