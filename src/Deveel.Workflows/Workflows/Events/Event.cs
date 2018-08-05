using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class Event
    {
        public Event(EventSource source, string name)
        {
            EventSource = source ?? throw new ArgumentNullException(nameof(source));
            Name = name;
        }

        public string Name { get; }

        public EventType EventType => EventSource.EventType;

        public EventSource EventSource { get; }

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
