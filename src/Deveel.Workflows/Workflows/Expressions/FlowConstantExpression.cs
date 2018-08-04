using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowConstantExpression : FlowExpression
    {
        internal FlowConstantExpression(object value)
        {
            Value = value;
        }

        public override FlowExpressionType NodeType => FlowExpressionType.Constant;

        public object Value { get; }

        public override Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<FlowExpression>(this);
        }
    }
}
