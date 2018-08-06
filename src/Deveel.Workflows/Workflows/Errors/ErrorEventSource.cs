using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventSource : FlowEventSource
    {
        private IErrorHandler handler;
        private Dictionary<EventId, EventContext> events;
        private Dictionary<EventId, Task> waiters;

        public ErrorEventSource(IErrorHandler handler)
        {
            this.handler = handler;
            events = new Dictionary<EventId, EventContext>();
            waiters = new Dictionary<EventId, Task>();
        }

        public override EventType EventType => EventType.Error;

        protected override Task AttachContextAsync(EventContext context)
        {
            if (!events.ContainsKey(context.EventId))
            {
                waiters[context.EventId] = Task.Run(() => WaitForErrorAsync(context.EventId, context.EventHandler.EventName, context.CancellationToken));
                events.Add(context.EventId, context);
            }

            return Task.CompletedTask;
        }

        protected override Task DetachContextAsync(EventContext context)
        {
            var id = context.EventId;

            if (events.ContainsKey(id))
            {
                var task = waiters[id];
                task.Dispose();

                waiters.Remove(id);
                events.Remove(id);
            }

            return Task.CompletedTask;
        }

        private async Task WaitForErrorAsync(EventId eventId, string errorName, CancellationToken cancellationToken)
        {
            var error = await handler.CatchErrorAsync(eventId.ProcessId, eventId.InstanceKey, errorName, cancellationToken);

            if (error != null && events.TryGetValue(eventId, out EventContext errorEvent))
            {
                await errorEvent.FireAsync(error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var waiter in waiters.Values)
                {
                    waiter.Wait(200);
                    waiter.Dispose();
                }

                foreach (var e in events.Values)
                {
                    e.Dispose();
                }

                events.Clear();
            }

            handler = null;
            base.Dispose(disposing);
        }
    }
}
