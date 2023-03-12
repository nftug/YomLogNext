using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using YomLog.Infrastructure.Shared.DataModels;
using YomLog.Shared.Extensions;

namespace YomLog.Infrastructure.Shared.Extensions;

internal static class QueryableExtensions
{
    // Reference: https://stackoverflow.com/a/7265354
    public static IQueryable<TSource> OrderByKey<TSource>(
        this IQueryable<TSource> source,
        string? key,
        bool isDescending
    )
    {
        if (string.IsNullOrWhiteSpace(key)) return source;

        var property = typeof(TSource).GetProperty(key);
        if (property == null) throw new InvalidOperationException();

        var parameter = Expression.Parameter(typeof(TSource), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property!);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var resultExpression = Expression
            .Call(
                typeof(Queryable),
                isDescending ? "OrderByDescending" : "OrderBy",
                new Type[] { typeof(TSource), property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression)
            );

        return source.Provider.CreateQuery<TSource>(resultExpression);
    }

    public static Expression<Func<T, T>> DbSetPredicate<T>(this DbContext context)
        where T : class, IDataModel, new()
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var propBindings = context.Set<T>().EntityType.GetProperties()
            .Where(prop => prop.PropertyInfo != null)
            .Select(prop => Expression.Bind(
                    prop.PropertyInfo!,
                    Expression.MakeMemberAccess(parameter, prop.PropertyInfo!)
                ));
        return propBindings.GetSelector<T, T>(parameter);
    }

    public static WithQuery<T, T> SetWithProperties<T>(this DbContext context)
        where T : class, IDataModel, new()
    {
        return new(context.DbSetPredicate<T>(), context.Set<T>());
    }
}
