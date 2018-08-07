using System;

namespace Deveel.Workflows
{
    public class Transaction : SubProcess
    {
        public Transaction(string id) : base(id)
        {
        }

        private void NodeFailed(FlowNode node, NodeContext context)
        {
            // TODO: throw the failure
        }

        protected override void OnNodeAttached(FlowNode node)
        {
            node.AttachFailCallback(NodeFailed);
        }

        protected override void OnNodeDetached(FlowNode node)
        {
            node.DetachFailCallback(NodeFailed);
        }
    }
}
