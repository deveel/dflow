using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowGroupExpression : FlowExpression
    {
        internal FlowGroupExpression(FlowExpression expression)
        {
            Expression = expression;
        }

        public  FlowExpression Expression { get; }

        public override FlowExpressionType NodeType => FlowExpressionType.Group;

        public override Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
