using YomLog.Shared.Exceptions;

namespace YomLog.Shared.ValueObjects;

public class Title : ValueObject<Title>
{
    public readonly int MaxTitleLength = 200;

    public string Value { get; }

    public Title(string? value)
    {
        if (value == null)
            throw new ArgumentNullException(value);

        if (value.Length > MaxTitleLength)
            throw new EntityValidationException(nameof(Title), $"The length of the field is too long.");

        Value = value;
    }

    protected override bool EqualsCore(Title other) => Value == other.Value;
}

public class TitleAttribute : ValueObjectAttributeBase<Title, string?>
{
    protected override Title CreateValueObject(string? value) => new(value);
}
