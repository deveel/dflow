using System;
using System.Threading.Tasks;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows
{
    public static class ConditionExtensions
    {
        public static async Task<bool> IsTrueAsync(this FlowExpression condition, IExecutionContext context)
        {
            var result = await condition.ReduceAsync(context);
            if (result.NodeType != FlowExpressionType.Constant)
                throw new InvalidOperationException();

            return (bool) ((FlowConstantExpression) result).Value;
        }
    }
}
