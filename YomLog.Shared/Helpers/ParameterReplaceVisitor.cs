using System.Linq.Expressions;

namespace YomLog.Shared.Helpers;

public class ParameterReplaceVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _originalParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplaceVisitor(
        ParameterExpression originalParameter,
        ParameterExpression newParameter
    )
    {
        _originalParameter = originalParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node == _originalParameter)
        {
            return _newParameter;
        }

        return base.VisitParameter(node);
    }
}
