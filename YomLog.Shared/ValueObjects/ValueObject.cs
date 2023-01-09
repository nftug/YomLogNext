namespace YomLog.Shared.ValueObjects;

public abstract class ValueObject<T> where T : ValueObject<T>
{
    protected abstract bool EqualsCore(T other);

    public override bool Equals(object? obj)
    {
        var vo = obj as T;
        if (vo == null) return false;

        return EqualsCore(vo);
    }

    public static bool operator ==(ValueObject<T>? vo1, ValueObject<T>? vo2)
        => Equals(vo1, vo2);

    public static bool operator !=(ValueObject<T>? vo1, ValueObject<T>? vo2)
        => !Equals(vo1, vo2);

    public override int GetHashCode() => base.GetHashCode();
}