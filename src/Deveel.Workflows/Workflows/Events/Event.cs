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

        protected virtual EventContext CreateContext(ExecutionContext context)
        {
            return new EventContext(this, context);
        }

        internal virtual async Task<EventContext> CreateContextAsync(ExecutionContext context)
        {
            var eventContext = CreateContext(context);
            await EventSource.AttachAsync(eventContext);
            return eventContext;
        }
    }
}
