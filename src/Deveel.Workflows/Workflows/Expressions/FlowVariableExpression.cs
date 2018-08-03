using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowVariableExpression : FlowExpression
    {
        internal FlowVariableExpression(string variableName)
        {
            VariableName = variableName;
        }

        public string VariableName { get; }

        public override FlowExpressionType NodeType => FlowExpressionType.Variable;

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
