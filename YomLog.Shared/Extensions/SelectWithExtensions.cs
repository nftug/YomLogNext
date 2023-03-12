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
}

public record WithQuery<TSource, TResult>(Expression<Func<TSource, TResult>> Selector, IQueryable<TSource> Query);
