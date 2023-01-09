using System.ComponentModel.DataAnnotations;

namespace YomLog.Shared.Models;

public class MustInputAttribute : ValidationAttribute
{
    public bool IsPatch { get; init; } = false;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? _value = (string?)value;
        string[] memberNames = new[] { validationContext.MemberName! };

        if (IsPatch && _value == null) return ValidationResult.Success;

        if (string.IsNullOrWhiteSpace(_value))
            return new ValidationResult("Empty field is not allowed.", memberNames);

        return ValidationResult.Success;
    }
}
