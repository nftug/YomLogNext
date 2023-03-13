using System.Text.RegularExpressions;
using YomLog.Shared.ValueObjects;

namespace YomLog.Domain.Books.ValueObjects;

public partial class AuthorName : ValueObject<AuthorName>
{
    public string Value { get; }

    public AuthorName(string name)
    {
        string jpPattern = @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+";
        name = name.Replace("\u3000", " ");
        name = new Regex($@"({jpPattern}) ({jpPattern})").Replace(name, "$1$2");
        Value = name;
    }

    protected override bool EqualsCore(AuthorName other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
