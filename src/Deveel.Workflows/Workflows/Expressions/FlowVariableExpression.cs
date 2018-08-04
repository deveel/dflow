using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Variables;

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

        public override async Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var variable = await context.FindVariableAsync(VariableName);
            if (variable == null)
                throw new FlowExpressionException($"Could not resolve variable '{VariableName}' in the context");

            return Constant(variable);
        }
    }
}
