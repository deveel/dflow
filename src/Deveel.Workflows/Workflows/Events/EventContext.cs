using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class EventContext<TEvent> : IEventContext
        where TEvent : IEvent
    {
        private Action<IEvent, ExecutionContext> callbacks;
        private IServiceScope scope;

        public EventContext(TEvent @event, ExecutionContext parent)
        {
            Event = @event;
            Parent = parent;
            scope = parent.CreateScope();

            var processId = parent.Process.Id;
            var instanceId = parent.Process.InstanceId;
            EventId = new EventId(processId, instanceId, @event.Name);
        }

        IContext IContext.Parent { get; }

        public ExecutionContext Parent { get; }

        IEvent IEventContext.Event => Event;

        public TEvent Event { get; }

        public EventId EventId { get; }
        
        void IEventContext.Attach(Action<IEvent, ExecutionContext> callback)
        {
            Attach(callback);
        }

        protected virtual void Attach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks == null)
            {
                callbacks = callback;
            }
            else
            {
                callbacks = (Action<IEvent, ExecutionContext>)Delegate.Combine(callbacks, callback);
            }
        }

        void IEventContext.Detach(Action<IEvent, ExecutionContext> callback)
        {
            Detach(callback);
        }

        protected virtual void Detach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks != null)
                callbacks = (Action<IEvent, ExecutionContext>)Delegate.Remove(callbacks, callback);
        }

        public object GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        public virtual Task FireAsync()
        {
            callbacks?.Invoke(Event, Parent);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (Event != null)
                Event.Source.DetachAsync(this).Wait();

            scope?.Dispose();
        }
    }
}
