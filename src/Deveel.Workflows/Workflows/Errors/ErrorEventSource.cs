using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventSource : IEventSource
    {
        private IErrorSignal errorSignal;
        private Dictionary<EventId, IEventContext> events;
        private Dictionary<EventId, Task> waiters;
        private CancellationTokenSource tokenSource;

        public ErrorEventSource(IErrorSignal errorSignal)
        {
            this.errorSignal = errorSignal;
            events = new Dictionary<EventId, IEventContext>();
            waiters = new Dictionary<EventId, Task>();
            tokenSource = new CancellationTokenSource();
        }

        public EventType EventType => EventType.Error;

        Task IEventSource.AttachAsync(IEventContext context)
        {
            Attach((EventContext<ErrorEvent>)context);
            return Task.CompletedTask;
        }

        internal void Attach(EventContext<ErrorEvent> context)
        {
            if (!events.ContainsKey(context.EventId))
            {
                waiters[context.EventId] = Task.Run(() => WaitForErrorAsync(context.EventId, context.Event.Name), tokenSource.Token);
                events.Add(context.EventId, context);
            }
        }

        Task IEventSource.DetachAsync(IEventContext context)
        {
            Detach((EventContext<ErrorEvent>) context);
            return Task.CompletedTask;
        }

        private void Detach(EventContext<ErrorEvent> context)
        {
            var id = context.EventId;

            if (events.ContainsKey(id))
            {
                var task = waiters[id];
                task.Dispose();

                waiters.Remove(id);
                events.Remove(id);
            }
        }

        private async Task WaitForErrorAsync(EventId eventId, string errorName)
        {
            var error = await errorSignal.WaitForErrorAsync(eventId.ProcessId, eventId.InstanceKey, errorName, tokenSource.Token);

            if (error != null && events.TryGetValue(eventId, out IEventContext errorEvent))
            {
                await errorEvent.FireAsync();
            }
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            
            foreach(var e in events.Values)
            {
                e.Dispose();
            }

            events.Clear();
            errorSignal = null;
        }
    }
}
