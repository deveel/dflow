using System;
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

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
