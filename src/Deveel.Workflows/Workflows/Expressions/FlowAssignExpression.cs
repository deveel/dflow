using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        internal override Task<FlowExpression> ReduceAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
