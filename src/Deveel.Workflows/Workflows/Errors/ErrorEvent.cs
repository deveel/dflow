using Deveel.Workflows.Events;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEvent : IEvent
    {
        private Action<ErrorEvent, ExecutionContext> callbacks;

        private string processId;
        private string instanceId;

        public ErrorEvent(ErrorEventSource source, string name)
        {
            Name = name;
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public string Name { get; }

        IEventSource IEvent.Source => Source;

        public ErrorEventSource Source { get; }

        Task<IEventContext> IEvent.CreateContextAsync(ExecutionContext context)
        {
            var eventContext = new EventContext<ErrorEvent>(this, context);
            Source.Attach(eventContext);
            return Task.FromResult<IEventContext>(eventContext);
        }

        public void Dispose()
        {
            callbacks = null;
        }
    }
}
