using System.Linq.Expressions;

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
