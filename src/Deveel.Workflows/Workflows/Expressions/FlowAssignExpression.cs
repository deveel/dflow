using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows.Expressions
{
    public sealed class FlowAssignExpression : FlowExpression
    {
        internal FlowAssignExpression(string variableName, FlowExpression value)
        {
            VariableName = variableName;
            Value = value;
        }

        public string VariableName { get; }

        public FlowExpression Value { get; }

        public override FlowExpressionType NodeType => FlowExpressionType.Assign;

        public override Task<FlowExpression> ReduceAsync(IContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
