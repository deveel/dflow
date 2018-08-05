using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class FlowEventHandler
    {
        public FlowEventHandler(FlowEventSource source, string eventName)
        {
            EventSource = source ?? throw new ArgumentNullException(nameof(source));
            EventName = eventName;
        }

        public string EventName { get; }

        public EventType EventType => EventSource.EventType;

        public FlowEventSource EventSource { get; }

        protected virtual EventContext OnCreateContext(ExecutionContext context)
        {
            return new EventContext(this, context);
        }

        internal virtual EventContext CreateContext(ExecutionContext context)
        {
            return OnCreateContext(context);
        }
    }
}
