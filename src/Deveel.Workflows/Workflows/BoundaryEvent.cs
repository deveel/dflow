using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public sealed class BoundaryEvent : IDisposable
    {
        private IEvent source;
        private IEventContext eventContext;
        private Action<IEvent, ExecutionContext> callback;
        private Activity attachedActivity;

        public BoundaryEvent(IEvent source, FlowNode node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            this.source = source;
        }

        public FlowNode Node { get; }

        public bool Interrupting { get; set; }

        public EventType EventType => source.Source.EventType;

        internal async Task InitAsync(ExecutionContext context)
        {
            eventContext = await source.CreateContextAsync(context);
            eventContext.Attach(callback);
        }

        internal void AttachTo(Activity activity)
        {
            attachedActivity = activity;
            callback = async (e,c) => await ReactAsync(e, c);
        }

        internal void DetachFrom(Activity activity)
        {
            if (activity == attachedActivity)
            {
                attachedActivity = null;
            }
        }

        private Task ReactAsync(IEvent e, ExecutionContext context)
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
