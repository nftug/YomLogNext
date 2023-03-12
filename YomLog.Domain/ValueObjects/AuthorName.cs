using System.Text.RegularExpressions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.ValueObjects;

public partial class AuthorName : ValueObject<AuthorName>
{
    public string Value { get; }

    public AuthorName(string name)
    {
        name = name.Replace("\u3000", " ");
        name = JapaneseRegex().Replace(name, "$1$2");
        Value = name;
    }

    protected override bool EqualsCore(AuthorName other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    [GeneratedRegex("([亜-熙ぁ-んァ-ヶ]) ([亜-熙ぁ-んァ-ヶ])")]
    private static partial Regex JapaneseRegex();
}
