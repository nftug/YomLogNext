using System.ComponentModel.DataAnnotations;
using YomLog.Shared.Exceptions;

namespace YomLog.Shared.ValueObjects;

public abstract class ValueObjectAttributeBase<T, TValue> : ValidationAttribute
    where T : ValueObject<T>
{
    public bool IsPatch { get; init; } = false;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        TValue? _value = (TValue?)value;
        string[] memberNames = new[] { validationContext.MemberName! };

        if (IsPatch && _value == null) return ValidationResult.Success;

        try
        {
            var item = CreateValueObject(_value);
            return ValidationResult.Success;
        }
        catch (ModelErrorException e)
        {
            return new ValidationResult(e.Message, memberNames);
        }
    }

    protected abstract T CreateValueObject(TValue? value);
}