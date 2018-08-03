using System;

namespace Deveel.Workflows
{
    public abstract class OutGatewayFlow : IGatewayFlow
    {
        protected OutGatewayFlow(FlowNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public FlowNode Node { get; }

        string IGatewayFlow.NodeRef => Node.Id;
    }
}
