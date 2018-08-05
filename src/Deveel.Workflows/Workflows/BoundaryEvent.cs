using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public sealed class BoundaryEvent : IDisposable
    {
        private Event source;
        private EventContext eventContext;
        private Action<Event, ExecutionContext, object> callback;
        private Activity attachedActivity;

        public BoundaryEvent(Event source, FlowNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            this.source = source;
        }

        public FlowNode Node { get; }

        public bool Interrupting { get; set; }

        public EventType EventType => source.EventSource.EventType;

        internal async Task InitAsync(ExecutionContext context)
        {
            eventContext = await source.CreateContextAsync(context);
            eventContext.Attach(callback);
        }

        internal void AttachTo(Activity activity)
        {
            attachedActivity = activity;
            callback = async (e,c, s) => await ReactAsync(e, c, s);
        }

        internal void DetachFrom(Activity activity)
        {
            if (activity == attachedActivity)
            {
                attachedActivity = null;
            }
        }

        private Task ReactAsync(Event e, ExecutionContext context, object state)
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
