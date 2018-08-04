using System.Threading;
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

        public override async Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            var constant = await Operand.ReduceAsync(context, cancellationToken);
            if (constant.NodeType != FlowExpressionType.Constant)
                throw new FlowExpressionException("It was not possible to reduce to a constant");

            return Constant(UnaryOperator.Compute(NodeType, ((FlowConstantExpression) constant).Value));
        }
    }
}
