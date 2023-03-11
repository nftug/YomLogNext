using System.Reflection;

namespace YomLog.BlazorShared.Extensions;

public static class ObjectExtensions
{
    public static T ToObject<T>(this IDictionary<string, dynamic?> source)
        where T : class, new()
    {
        var someObject = new T();
        var someObjectType = someObject.GetType();

        foreach (var item in source)
        {
            someObjectType?.GetProperty(item.Key)?.SetValue(someObject, item.Value, null);
        }
        return someObject;
    }

    public static T ToObject<T>(this IDictionary<string, string?> source)
        where T : new()
    {
        var someObject = new T();
        var someObjectType = someObject.GetType();

        foreach (var item in source)
        {
            var property = someObjectType?.GetProperty(item.Key);
            var propertyType = property?.PropertyType;

            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                bool parsed = int.TryParse(item.Value, out int result);
                if (parsed) property?.SetValue(someObject, result, null);
            }
            else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
            {
                bool parsed = Guid.TryParse(item.Value, out Guid result);
                if (parsed) property?.SetValue(someObject, result, null);
            }
            else
            {
                property?.SetValue(someObject, item.Value, null);
            }
        }
        return someObject;
    }

    public static Dictionary<string, string?> AsDictionary<T>(
        this T source,
        BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance
    ) where T : notnull
        => source.GetType().GetProperties(bindingAttr)
            .Select(x => new { x.Name, Value = x.GetValue(source, null) })
            .Where(x => x.Value != null)
            .ToDictionary(x => x.Name, x => x.Value?.ToString());
}
