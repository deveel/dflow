using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows
{
    public sealed class ConditionalGatewayFlow : OutGatewayFlow
    {
        public ConditionalGatewayFlow(FlowNode node, FlowExpression condition) : base(node)
        {
            Condition = condition;
        }

        public ConditionalGatewayFlow(FlowNode node)
            : this(node, null)
        {
        }

        public FlowExpression Condition { get; }
    }
}
