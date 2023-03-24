using System.Linq.Expressions;
using System.Reflection;

namespace YomLog.Shared.Helpers;

public class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _originalParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplaceVisitor(ParameterExpression originalParameter, ParameterExpression newParameter)
    {
        _originalParameter = originalParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
        => node == _originalParameter ? _newParameter : base.VisitParameter(node);
}

public class MemberInitBindingsVisitor : ExpressionVisitor
{
    public ICollection<MemberBinding>? MemberBindings { get; private set; }
    public ParameterExpression? Parameter { get; private set; }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
        MemberBindings = node.Bindings;
        return base.VisitMemberInit(node);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        Parameter = node;
        return base.VisitParameter(node);
    }

    public ICollection<MemberBinding>? GetMemberInitBindings(Expression node)
    {
        Visit(node);
        return MemberBindings;
    }
}

public class BindingReplaceVisitor : ExpressionVisitor
{
    private readonly MemberInfo _member;
    private readonly Expression _parentExpression;

    public BindingReplaceVisitor(MemberInfo member, Expression parentExpression)
    {
        _member = member;
        _parentExpression = parentExpression;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Member.DeclaringType != _member.DeclaringType)
        {
            var parent =
                node.Expression is MemberExpression memberExpression
                ? Expression.MakeMemberAccess(memberExpression.Expression, memberExpression.Member)
                : Expression.MakeMemberAccess(_parentExpression, _member);
            node = Expression.MakeMemberAccess(parent, node.Member);
        }
        return base.VisitMember(node);
    }
}
