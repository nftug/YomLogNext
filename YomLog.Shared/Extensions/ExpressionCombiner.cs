using System.Linq.Expressions;
using YomLog.Shared.Helpers;
using YomLog.Shared.Models;

namespace YomLog.Shared.Extensions;

public static class ExpressionCombiner
{
    public static Expression<Func<T, bool>> OrElse<T>(params Expression<Func<T, bool>>[] expressions)
        => expressions.AsEnumerable().OrElse();

    public static Expression<Func<T, bool>> OrElse<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        => Combine(expressions, CombineMode.OrElse);

    public static Expression<Func<T, bool>> And<T>(params Expression<Func<T, bool>>[] expressions)
        => expressions.AsEnumerable().And();

    public static Expression<Func<T, bool>> And<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        => Combine(expressions, CombineMode.And);

    private static Expression<Func<T, bool>> Combine<T>(IEnumerable<Expression<Func<T, bool>>> expressions, CombineMode mode)
    {
        if (!expressions.Any())
        {
            throw new ArgumentException(
                $"parameter [{nameof(expressions)}] is empty.", nameof(expressions)
            );
        }

        var lambda = expressions.First();
        var body = lambda.Body;
        var parameter = lambda.Parameters[0];

        foreach (var expression in expressions.Skip(1))
        {
            var visitor = new ParameterReplaceVisitor(expression.Parameters[0], parameter);
            if (mode == CombineMode.And)
                body = Expression.And(body, visitor.Visit(expression.Body));
            if (mode == CombineMode.OrElse)
                body = Expression.OrElse(body, visitor.Visit(expression.Body));
        }

        lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
        return lambda;
    }
}