using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public sealed class BoundaryEvent : IDisposable
    {
        private FlowEventHandler source;
        private EventContext eventContext;
        private Action<ExecutionContext, object> callback;
        private Activity attachedActivity;

        public BoundaryEvent(FlowEventHandler source, FlowNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            this.source = source;
        }

        public FlowNode Node { get; }

        public bool Interrupting { get; set; }

        public EventType EventType => source.EventSource.EventType;

        internal void Init(ExecutionContext context)
        {
            eventContext = source.CreateContext(context);
            eventContext.Attach(callback);
        }

        internal void AttachTo(Activity activity)
        {
            attachedActivity = activity;
            callback = async (c, s) => await ReactAsync(c, s);
        }

        internal Task BeginAsync()
        {
            return eventContext.BeginAsync();
        }

        internal void DetachFrom(Activity activity)
        {
            if (activity == attachedActivity)
            {
                attachedActivity = null;
            }
        }

        private Task ReactAsync(ExecutionContext context, object state)
        {
            if (Interrupting)
            {
                attachedActivity.Interupt();
            }

            return Node.ExecuteAsync(context);
        }

        public void Dispose()
        {
            eventContext?.Detach(callback);
            eventContext?.Dispose();
        }
    }
}
