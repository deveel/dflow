using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventSource : EventSource
    {
        private IErrorHandler handler;
        private Dictionary<EventId, EventContext> events;
        private Dictionary<EventId, Task> waiters;

        public ErrorEventSource(IErrorHandler handler, string id, string errorName, string errorCode)
            : base(id) {
            this.handler = handler;
            ErrorName = errorName;
            ErrorCode = errorCode;
            events = new Dictionary<EventId, EventContext>();
            waiters = new Dictionary<EventId, Task>();
        }

        public override EventType EventType => EventType.Error;

        public string ErrorName { get; }

        public  string ErrorCode { get; }

        protected override Task AttachContextAsync(EventContext context)
        {
            if (!events.ContainsKey(context.EventId))
            {
                waiters[context.EventId] = Task.Run(() => WaitForErrorAsync(context.EventId, context.CancellationToken));
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

        private async Task WaitForErrorAsync(EventId eventId, CancellationToken cancellationToken)
        {
            var error = await handler.CatchErrorAsync(eventId.ProcessId, eventId.InstanceKey, ErrorCode, cancellationToken);

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
