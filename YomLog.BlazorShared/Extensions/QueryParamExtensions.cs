namespace YomLog.BlazorShared.Extensions;

public static class QueryParamExtension
{
    public static int? ParseAsIntParam(this string? value, Func<int, bool, int?> func)
    {
        bool canParse = int.TryParse(value, out int _value);
        return func(_value, canParse);
    }

    public static int ParsePageAsStartIndex(this string? value, int limit)
    {
        var page = (int)value.ParseAsIntParam((x, _) => x > 0 ? x : 1)!;
        return (page - 1) * limit;
    }
}
