using Deveel.Workflows.Events;
using System;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEvent : IEvent
    {
        private Action<ErrorEvent, ExecutionContext> callbacks;
        private ExecutionContext attachedContext;

        private string processId;
        private string instanceId;

        public ErrorEvent(ErrorEventSource source, string name)
        {
            Name = name;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Source.Attach(this);
        }

        public string Name { get; }

        IEventSource IEvent.Source => Source;

        public ErrorEventSource Source { get; }

        void IEvent.Attach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks == null)
            {
                callbacks = (Action<ErrorEvent, ExecutionContext>) callback;
            } else
            {
                callbacks = (Action<ErrorEvent, ExecutionContext>)Delegate.Combine(callbacks, callback);
            }
        }

        void IEvent.AttachToContext(ExecutionContext context)
        {
            attachedContext = context;
            processId = context.Process.Id;
            instanceId = context.Process.InstanceId;
        }

        void IEvent.Detach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks != null)
                callbacks = (Action<ErrorEvent, ExecutionContext>)Delegate.Remove(callbacks, callback);
        }

        internal void HandleError(ThrownError error)
        {
            if (processId == error.ProcessId &&
                instanceId == error.InstanceId)
            callbacks?.Invoke(this, attachedContext);
        }

        public void Dispose()
        {
            Source.Detach(this);
            attachedContext = null;
            callbacks = null;
        }
    }
}
