﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public class EventContext : ContextBase {
        private Action<NodeContext, IEventArgument> callbacks;
        private AutoResetEvent fireEvent;
        private object lastState;

        public EventContext(NodeContext parent, EventSource source)
            : base(parent)
        {
            EventSource = source;

            var processId = parent.Process.Id;
            var instanceId = parent.Process.InstanceKey;
            EventId = new EventId(processId, instanceId, source.EventName);

            fireEvent = new AutoResetEvent(false);
        }

        public EventSource EventSource { get; }

        public EventId EventId { get; }

        public NodeContext Parent => (NodeContext)ParentContext;
        
        internal void Attach(Action<NodeContext, IEventArgument> callback)
        {
            OnAttach(callback);
        }

        protected virtual void OnAttach(Action<NodeContext, IEventArgument> callback)
        {
            if (callbacks == null)
            {
                callbacks = callback;
            }
            else
            {
                callbacks = (Action<NodeContext, IEventArgument>)Delegate.Combine(callbacks, callback);
            }
        }

        internal void Detach(Action<NodeContext, IEventArgument> callback)
        {
            OnDetach(callback);
        }

        protected virtual void OnDetach(Action<NodeContext, IEventArgument> callback)
        {
            if (callbacks != null)
                callbacks = (Action<NodeContext, IEventArgument>)Delegate.Remove(callbacks, callback);
        }

        internal Task BeginAsync()
        {
            return EventSource.AttachAsync(this);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (EventSource != null)
                    EventSource.DetachAsync(this).Wait();

            }

            base.Dispose(disposing);
        }
    }
}
