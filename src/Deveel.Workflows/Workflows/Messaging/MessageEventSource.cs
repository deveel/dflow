using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Messaging {
    public sealed class MessageEventSource : EventSource {
        private IMessageReceiver receiver;
        private Dictionary<EventId, Task> waiters;
        private Dictionary<EventId, EventContext> events;

        public MessageEventSource(string id, IMessageReceiver receiver, MessageSubscription subscription)
            : base(id) {
            this.receiver = receiver;
            Subscription = subscription;

            waiters = new Dictionary<EventId, Task>();
            events = new Dictionary<EventId, EventContext>();
        }

        public override EventType EventType => EventType.Message;

        public MessageSubscription Subscription { get; }

        protected override Task AttachContextAsync(EventContext context) {
            if (!events.ContainsKey(context.EventId)) {
                waiters[context.EventId] =
                    Task.Run(async () => await ReceiveAsync(context.EventId, context.CancellationToken));
                events.Add(context.EventId, context);
            }

            return Task.CompletedTask;
        }

        private async Task ReceiveAsync(EventId eventId, CancellationToken cancellationToken) {
            var message = await receiver.ReceiveAsync(Subscription, cancellationToken);

            if (events.TryGetValue(eventId, out var context))
                await context.FireAsync(message);
        }

        protected override Task DetachContextAsync(EventContext context) {
            if (events.ContainsKey(context.EventId)) {
                var waiter = waiters[context.EventId];
                waiter.Dispose();

                waiters.Remove(context.EventId);
                events.Remove(context.EventId);
            }

            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                foreach (var waiter in waiters.Values)
                    waiter.Dispose();

                waiters.Clear();
                events.Clear();
            }
        }
    }
}