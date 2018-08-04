using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows
{
    static class ExpressionExtensions
    {
        public static Task<bool> IsTrueAsync(this FlowExpression condition, IContext context, CancellationToken cancellationToken)
        {
            return condition.ReduceToAsync<bool>(context, cancellationToken);
        }

        public static async Task<T> ReduceToAsync<T>(this FlowExpression expression, IContext context, CancellationToken cancellationToken)
        {
            var result = await expression.ReduceAsync(context, cancellationToken);
            if (result.NodeType != FlowExpressionType.Constant)
                throw new FlowExpressionException("The expression does not resolve to a constant");

            return (T) Convert.ChangeType(((FlowConstantExpression) result).Value, typeof(T));
        }
    }
}
