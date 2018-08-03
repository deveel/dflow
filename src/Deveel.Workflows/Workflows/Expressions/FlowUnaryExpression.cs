using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowUnaryExpression : FlowExpression
    {
        internal FlowUnaryExpression(FlowExpressionType nodeType, FlowExpression operand)
        {
            NodeType = nodeType;
            Operand = operand;
        }

        public override FlowExpressionType NodeType { get; }

        public FlowExpression Operand { get; }

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
