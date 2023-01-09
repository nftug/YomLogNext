using System.Linq.Expressions;
using YomLog.Shared.Extensions;

namespace YomLog.Shared.Helpers;

public static class ExpressionHelper
{
    public static Expression<Func<T, bool>> Equal<T, U>(
        Expression<Func<T, U>> expression,
        U? value
    )
    {
        var (parameter, property) = expression.GetParameterAndProperty();
        var constant = Expression.Constant(value);
        var body = Expression.Equal(property, constant);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
