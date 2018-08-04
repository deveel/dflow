using System;

namespace Deveel.Workflows.Events
{
    public interface IEventSource : IDisposable
    {
        EventType EventType { get; }

        void Attach(IEvent @event);
    }
}
