using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public sealed class BoundaryEvent
    {
        private IEvent source;
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

        internal void Init(ExecutionContext context)
        {
            source.AttachToContext(context);
        }

        internal void AttachTo(Activity activity)
        {
            attachedActivity = activity;
            callback = async (e,c) => await ReactAsync(e, c);
            source.Attach(callback);
        }

        internal void DetachFrom(Activity activity)
        {
            if (activity == attachedActivity)
                source.Detach(callback);
        }

        private Task ReactAsync(IEvent e, ExecutionContext context)
        {
            if (Interrupting)
            {
                attachedActivity.Interupt();
            }

            return Node.ExecuteAsync(context);
        }
    }
}
