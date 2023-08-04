using Pluralize.NET.Core;

namespace System;

public static partial class StringExtensions
{
    private static Pluralizer Pluralizer => new();

    public static string Pluralize(this string word) => Pluralizer.Pluralize(word);

    public static string Singularize(this string word) => Pluralizer.Singularize(word);
}
