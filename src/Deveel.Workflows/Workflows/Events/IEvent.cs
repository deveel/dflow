using System;

namespace Deveel.Workflows.Events
{
    public interface IEvent : IDisposable
    {
        string Name { get; }

        IEventSource Source { get; }

        void AttachToContext(ExecutionContext context);

        void Attach(Action<IEvent, ExecutionContext> callback);

        void Detach(Action<IEvent, ExecutionContext> callback);
    }
}
