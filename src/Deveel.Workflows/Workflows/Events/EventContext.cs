using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class EventContext : IContext {
        private Action<ExecutionContext, object> callbacks;
        private IServiceScope scope;
        private AutoResetEvent fireEvent;
        private object lastState;

        public EventContext(FlowEventHandler @event, ExecutionContext parent)
        {
            Event = @event;
            Parent = parent;
            scope = parent.CreateScope();

            var processId = parent.Process.Id;
            var instanceId = parent.Process.InstanceId;
            EventId = new EventId(processId, instanceId, @event.EventName);

            fireEvent = new AutoResetEvent(false);
        }

        IContext IContext.Parent { get; }

        public ExecutionContext Parent { get; }

        public FlowEventHandler Event { get; }

        public EventId EventId { get; }

        public CancellationToken CancellationToken => Parent.CancellationToken;
        
        internal void Attach(Action<ExecutionContext, object> callback)
        {
            OnAttach(callback);
        }

        protected virtual void OnAttach(Action<ExecutionContext, object> callback)
        {
            if (callbacks == null)
            {
                callbacks = callback;
            }
            else
            {
                callbacks = (Action<ExecutionContext, object>)Delegate.Combine(callbacks, callback);
            }
        }

        internal void Detach(Action<ExecutionContext, object> callback)
        {
            OnDetach(callback);
        }

        protected virtual void OnDetach(Action<ExecutionContext, object> callback)
        {
            if (callbacks != null)
                callbacks = (Action<ExecutionContext, object>)Delegate.Remove(callbacks, callback);
        }

        public object GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        internal Task BeginAsync()
        {
            return Event.EventSource.AttachAsync(this);
        }

        internal async Task FireAsync(object state)
        {
            await OnFired(state);

            fireEvent.Set();
        }

        protected virtual Task OnFired(object state)
        {
            callbacks?.Invoke(Parent, state);
            lastState = state;

            return Task.CompletedTask;
        }

        internal object Wait(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
                fireEvent.WaitOne(200);

            return lastState;
        }

        public void Dispose()
        {
            if (Event != null)
                Event.EventSource.DetachAsync(this).Wait();

            scope?.Dispose();
        }
    }
}
