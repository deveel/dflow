using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public sealed class BoundaryEvent : FlowNode
    {
        private Action<NodeContext, object> callback;
        private Activity attachedActivity;

        public BoundaryEvent(string id, EventSource source, FlowNode node)
            : base(id)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            EventSource = source;
        }

        public FlowNode Node { get; }

        public override FlowNodeType NodeType => FlowNodeType.BoundaryEvent;

        public bool Interrupting { get; set; }

        public EventSource EventSource { get; }

        public EventType EventType => EventSource.EventType;

        internal void AttachTo(Activity activity)
        {
            attachedActivity = activity;
            callback = async (c, s) => await ReactAsync(c, s);
        }

        internal void DetachFrom(Activity activity)
        {
            if (activity == attachedActivity)
            {
                attachedActivity = null;
            }
        }

        private Task ReactAsync(NodeContext context, object state)
        {
            if (Interrupting)
            {
                attachedActivity.Interupt();
            }

            return Node.ExecuteAsync(context);
        }

        protected override Task<object> CreateStateAsync(NodeContext context)
        {
            var eventContext = EventSource.NewEventContext(context);
            eventContext.Attach(callback);

            return Task.FromResult<object>(new BoundaryEventState(eventContext, callback));
        }

        protected override Task ExecuteNodeAsync(object state, NodeContext context)
        {
            var boundaryState = (BoundaryEventState)state;
            return boundaryState.EventContext.BeginAsync();
        }

        #region BoundaryEventState

        class BoundaryEventState : IDisposable
        {
            public BoundaryEventState(EventContext context, Action<NodeContext, object> callback)
            {
                EventContext = context;
                Callback = callback;
            }

            public EventContext EventContext { get; }

            public Action<NodeContext, object> Callback { get; }

            public void Dispose()
            {
                EventContext?.Detach(Callback);
                EventContext?.Dispose();
            }
        }

        #endregion
    }
}
