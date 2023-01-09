using System.Linq.Expressions;

namespace YomLog.Shared.Extensions;

public static class ExpressionExtensions
{
    public static (ParameterExpression parameter, MemberExpression property) GetParameterAndProperty<T, U>
        (this Expression<Func<T, U>> expression)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression? property = null;

        foreach (var name in expression.GetMemberNames().Reverse())
        {
            if (property == null)
                property = Expression.Property(parameter, name);
            else
                property = Expression.Property(property, name);
        }

        return (parameter!, property!);
    }

    public static IEnumerable<string> GetMemberNames<ObjectType, MemberType>
        (this Expression<Func<ObjectType, MemberType>> func)
    {
        var memberExp = func.Body as MemberExpression;
        while (memberExp != null)
        {
            var memberInfo = memberExp.Member;
            yield return memberInfo.Name;
            memberExp = memberExp.Expression as MemberExpression;
        }
    }
}
