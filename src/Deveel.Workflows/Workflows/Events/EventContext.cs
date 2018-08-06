using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class EventContext : IContext {
        private Action<ExecutionContext, IEventArgument> callbacks;
        private IServiceScope scope;
        private AutoResetEvent fireEvent;
        private object lastState;

        public EventContext(FlowEventHandler handler, ExecutionContext parent)
        {
            EventHandler = handler;
            Parent = parent;
            scope = parent.CreateScope();

            var processId = parent.Process.Id;
            var instanceId = parent.Process.InstanceId;
            EventId = new EventId(processId, instanceId, handler.EventName);

            fireEvent = new AutoResetEvent(false);
        }

        IContext IContext.Parent { get; }

        public ExecutionContext Parent { get; }

        public FlowEventHandler EventHandler { get; }

        public EventId EventId { get; }

        public CancellationToken CancellationToken => Parent.CancellationToken;
        
        internal void Attach(Action<ExecutionContext, object> callback)
        {
            OnAttach(callback);
        }

        protected virtual void OnAttach(Action<ExecutionContext, IEventArgument> callback)
        {
            if (callbacks == null)
            {
                callbacks = callback;
            }
            else
            {
                callbacks = (Action<ExecutionContext, IEventArgument>)Delegate.Combine(callbacks, callback);
            }
        }

        internal void Detach(Action<ExecutionContext, object> callback)
        {
            OnDetach(callback);
        }

        protected virtual void OnDetach(Action<ExecutionContext, IEventArgument> callback)
        {
            if (callbacks != null)
                callbacks = (Action<ExecutionContext, IEventArgument>)Delegate.Remove(callbacks, callback);
        }

        public object GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        internal Task BeginAsync()
        {
            return EventHandler.EventSource.AttachAsync(this);
        }

        internal async Task FireAsync(IEventArgument state)
        {
            await OnFired(state);

            fireEvent.Set();
        }

        protected virtual Task OnFired(IEventArgument state)
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
            if (EventHandler != null)
                EventHandler.EventSource.DetachAsync(this).Wait();

            scope?.Dispose();
        }
    }
}
