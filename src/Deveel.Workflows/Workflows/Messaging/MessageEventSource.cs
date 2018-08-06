using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Messaging
{
    public sealed class MessageEventSource : EventSource
    {
        private IMessageReceiver receiver;
        private Dictionary<MessageSubscription, Task> waiters;
        private Dictionary<MessageSubscription, EventContext> events;

        public MessageEventSource(IMessageReceiver receiver, string eventName, MessageSubscription subscription)
            : base(eventName)
        {
            this.receiver = receiver;
            Subscription = subscription;

            waiters = new Dictionary<MessageSubscription, Task>();
            events = new Dictionary<MessageSubscription, EventContext>();
        }

        public override EventType EventType => EventType.Message;

        public MessageSubscription Subscription { get; }

        protected override Task AttachContextAsync(EventContext context)
        {
            if (!waiters.ContainsKey(Subscription))
            {
                waiters[Subscription] = Task.Run(async () => await ReceiveAsync(Subscription, context.CancellationToken));
            }

            return Task.CompletedTask;
        }

        private async Task ReceiveAsync(MessageSubscription subscription, CancellationToken cancellationToken)
        {
            var message = await receiver.ReceiveAsync(subscription, cancellationToken);

            EventContext context;
            if (events.TryGetValue(subscription, out context))
                await context.FireAsync(message);
        }

        protected override Task DetachContextAsync(EventContext context)
        {
            if (events.ContainsKey(Subscription))
            {
                var waiter = waiters[Subscription];
                waiter.Dispose();

                waiters.Remove(Subscription);
                events.Remove(Subscription);
            }

            return Task.CompletedTask;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var waiter in waiters.Values)
                    waiter.Dispose();

                waiters.Clear();
                events.Clear();
            }
        }
    }
}
