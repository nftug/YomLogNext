using System.Text.RegularExpressions;
using Pluralize.NET.Core;

namespace YomLog.BlazorShared.Extensions;

public static partial class StringExtension
{
    // Reference: https://stackoverflow.com/a/1098039
    public static string SplitByCapitalLetter(this string stringToSplit)
        => CapitalLettersRegex().Replace(stringToSplit, " $1").Trim();

    [GeneratedRegex("((?<=\\p{Ll})\\p{Lu}|\\p{Lu}(?=\\p{Ll}))")]
    private static partial Regex CapitalLettersRegex();

    public static string Pluralize(this string word) => new Pluralizer().Pluralize(word);

    public static string Singularize(this string word) => new Pluralizer().Singularize(word);
}
