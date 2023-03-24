using System.Linq.Expressions;
using YomLog.Shared.Helpers;

namespace YomLog.Shared.Extensions;

public static class SelectWithExtensions
{
    public static WithQuery<TSource, TResult> SelectWith<TSource, TResult>(
        this IQueryable<TSource> query,
        Expression<Func<TSource, TResult>> selector,
        Expression<Func<TSource, TResult>>? previousSelector = null
    )
        where TResult : class, new()
    {
        if (previousSelector is null)
        {
            var visitor = new MemberInitBindingsVisitor();
            visitor.Visit(query.Expression);
            if (visitor.MemberBindings != null && visitor.Parameter != null)
                previousSelector = visitor.MemberBindings.GetSelector<TSource, TResult>(visitor.Parameter);
        }

        if (previousSelector != null)
            selector = previousSelector.CombineSelector(selector);

        return new(selector, query);
    }

    public static WithQuery<TSource, TResult> SelectWith<TSource, TResult>(
        this WithQuery<TSource, TResult> withQuery,
        Expression<Func<TSource, TResult>> selector
    )
        where TResult : class, new()
        => withQuery.Query.SelectWith(selector, withQuery.Selector);

    public static IQueryable<TResult> ToQueryable<TSource, TResult>(this WithQuery<TSource, TResult> withQuery)
        => withQuery.Query.Select(withQuery.Selector);

    public static IOrderedQueryable<TResult> OrderBy<TSource, TResult, TKey>(
        this WithQuery<TSource, TResult> withQuery,
        Expression<Func<TResult, TKey>> keySelector
    )
        => withQuery.ToQueryable().OrderBy(keySelector);

    public static List<TResult> ToList<TSource, TResult>(this WithQuery<TSource, TResult> withQuery)
        => withQuery.ToQueryable().ToList();

    public static IEnumerable<TResult> ToEnumerable<TSource, TResult>(this WithQuery<TSource, TResult> withQuery)
        => withQuery.ToQueryable().AsEnumerable();

    public static Expression<Func<TSource, TResult>> CombineSelector<TSource, TResult>(
        this Expression<Func<TSource, TResult>> selectorFirst,
        Expression<Func<TSource, TResult>> selectorSecond
    )
        where TResult : class, new()
    {
        var parameter = selectorFirst.Parameters[0];
        var parameterVisitor = new ParameterReplaceVisitor(selectorSecond.Parameters[0], parameter);
        selectorSecond = (Expression<Func<TSource, TResult>>)parameterVisitor.Visit(selectorSecond);

        var bindingVisitor = new MemberInitBindingsVisitor();
        var bindingsFirst = ((MemberInitExpression)bindingVisitor.Visit(selectorFirst.Body)).Bindings;
        var bindingsSecond = ((MemberInitExpression)bindingVisitor.Visit(selectorSecond.Body)).Bindings;

        return bindingsFirst!.Concat(bindingsSecond!).GetSelector<TSource, TResult>(parameter);
    }

    public static Expression<Func<TSource, TResult>> GetSelector<TSource, TResult>(
        this IEnumerable<MemberBinding> memberBindings,
        ParameterExpression parameter
    )
        where TResult : class, new()
        => Expression.Lambda<Func<TSource, TResult>>(
            Expression.MemberInit(Expression.New(typeof(TResult)), memberBindings),
            parameter
        );

    public static Expression<Func<TSource, TResult>> CombineBinding<TSource, TResult, TMember>(
    Expression<Func<TSource, TMember>> memberSelector,
    params Expression<Func<TMember, TMember>>[] selectors
)
    where TMember : class, new()
    where TResult : class, new()
    {
        var parameter = selectors.First().Parameters[0];
        selectors =
            selectors
                .Select(selector =>
                {
                    var parameterVisitor = new ParameterReplaceVisitor(selector.Parameters[0], parameter);
                    return (Expression<Func<TMember, TMember>>)parameterVisitor.Visit(selector);
                })
                .ToArray();

        var (_, property) = memberSelector.GetParameterAndProperty();
        var parameterParent = memberSelector.Parameters[0];
        var replaceVisitor = new BindingReplaceVisitor(property.Member, parameterParent);

        var memberInitBindings = selectors
            .Select(selector => (MemberInitExpression)replaceVisitor.Visit(selector.Body))
            .SelectMany(x => x.Bindings);

        var memberInit = Expression.MemberInit(Expression.New(typeof(TMember)), memberInitBindings);
        return new[] { Expression.Bind(property.Member, memberInit) }.GetSelector<TSource, TResult>(parameterParent);
    }

    public static WithQuery<TSource, TResult> SelectWith<TSource, TResult, TMember>(
        this WithQuery<TSource, TResult> withQuery,
        Expression<Func<TSource, TMember>> memberSelector,
        params Expression<Func<TMember, TMember>>[] memberExpressions
    )
        where TMember : class, new()
        where TResult : class, new()
        => withQuery.SelectWith(CombineBinding<TSource, TResult, TMember>(memberSelector, memberExpressions));

    public static WithQuery<TSource, TResult> SelectWith<TSource, TResult, TMember>(
        this IQueryable<TSource> query,
        Expression<Func<TSource, TMember>> memberSelector,
        params Expression<Func<TMember, TMember>>[] memberExpressions
    )
        where TMember : class, new()
        where TResult : class, new()
        => query.SelectWith(CombineBinding<TSource, TResult, TMember>(memberSelector, memberExpressions));

    public static IQueryable<T> SelectProperties<T>(this IQueryable<T> query) where T : class, new()
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var memberBindings = typeof(T).GetProperties()
            .Select(prop => Expression.Bind(prop, Expression.MakeMemberAccess(parameter, prop)));
        return query.Select(memberBindings.GetSelector<T, T>(parameter));
    }
}

public record WithQuery<TSource, TResult>(Expression<Func<TSource, TResult>> Selector, IQueryable<TSource> Query);
