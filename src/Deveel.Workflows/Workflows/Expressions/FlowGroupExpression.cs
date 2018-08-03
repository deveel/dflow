using System;
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

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
