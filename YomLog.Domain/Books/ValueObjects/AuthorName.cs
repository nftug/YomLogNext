using System.Text.RegularExpressions;

namespace YomLog.Domain.Books.ValueObjects;

public record AuthorName
{
    public string Value { get; init; } = string.Empty;

    public AuthorName(string name)
    {
        string jpPattern = @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+";
        Value = name.Replace("\u3000", " ");
        Value = new Regex($@"({jpPattern}) ({jpPattern})").Replace(name, "$1$2");
    }
}
