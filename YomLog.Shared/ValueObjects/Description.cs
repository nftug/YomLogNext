using YomLog.Shared.Exceptions;

namespace YomLog.Shared.ValueObjects;

public record Description
{
    public readonly int MaxContentLength = 500;

    public string? Value { get; }

    public Description(string? value)
    {
        if (value?.Length > MaxContentLength)
            throw new EntityValidationException(nameof(Description), $"The length of the field is too long.");

        Value = value;
    }
}

public class DescriptionAttribute : ValueObjectAttributeBase<Description, string?>
{
    protected override Description CreateValueObject(string? value) => new(value);
}
