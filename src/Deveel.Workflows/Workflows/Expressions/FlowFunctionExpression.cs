using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowFunctionExpression : FlowExpression
    {
        internal FlowFunctionExpression(string fucntionName, FlowExpression[] arguments)
        {
            FucntionName = fucntionName;
            Arguments = arguments;
        }

        public override FlowExpressionType NodeType => FlowExpressionType.Function;

        public string FucntionName { get; }

        public FlowExpression[] Arguments { get; }

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
