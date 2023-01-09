using YomLog.Shared.ValueObjects;

namespace YomLog.Shared.DTOs;

public class ItemCountDTO : ValueObject<ItemCountDTO>
{
    public int Count { get; init; }

    protected override bool EqualsCore(ItemCountDTO other) => Count == other.Count;
}
