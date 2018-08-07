using System;
using System.Threading.Tasks;

namespace Deveel.Workflows {
    public abstract class FlowNode {
        private Action<FlowNode, NodeContext> failCallback;

        protected FlowNode(string id) {
            Id = id;
        }

        public string Id { get; }

        public abstract FlowNodeType NodeType { get; }

        protected virtual Task<object> CreateStateAsync(NodeContext context) {
            return Task.FromResult<object>(null);
        }

        internal Task<object> CallCreateStateAsync(NodeContext context)
            => CreateStateAsync(context);

        internal async Task ExecuteAsync(NodeContext context) {
            var state = await CreateStateAsync(context);
            await ExecuteNodeAsync(state, context);
        }

        internal void AttachFailCallback(Action<FlowNode, NodeContext> callback) {
            failCallback = (Action<FlowNode, NodeContext>) Delegate.Combine(failCallback, callback);
        }

        internal void DetachFailCallback(Action<FlowNode, NodeContext> callback) {
            if (failCallback != null)
                failCallback = (Action<FlowNode, NodeContext>) Delegate.Remove(failCallback, callback);
        }

        internal void OnExecutionFailed(NodeContext context) {
            failCallback?.Invoke(this, context);
        }

        protected abstract Task ExecuteNodeAsync(object state, NodeContext context);

        internal Task CallExecuteNodeAsync(object state, NodeContext context) {
            return ExecuteNodeAsync(state, context);
        }
    }
}