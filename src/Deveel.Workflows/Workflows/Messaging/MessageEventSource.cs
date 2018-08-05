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

        public MessageEventSource(IMessageReceiver receiver)
        {
            this.receiver = receiver;
            waiters = new Dictionary<MessageSubscription, Task>();
            events = new Dictionary<MessageSubscription, EventContext>();
        }

        public override EventType EventType => EventType.Message;

        protected override Task AttachContextAsync(EventContext context)
        {
            var messageEvent = (MessageEvent)context.Event;

            if (!waiters.ContainsKey(messageEvent.Subscription))
            {
                waiters[messageEvent.Subscription] = Task.Run(async () => await ReceiveAsync(messageEvent.Subscription, context.CancellationToken));
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
            var messageEvent = (MessageEvent)context.Event;

            if (events.ContainsKey(messageEvent.Subscription))
            {
                var waiter = waiters[messageEvent.Subscription];
                waiter.Dispose();

                waiters.Remove(messageEvent.Subscription);
                events.Remove(messageEvent.Subscription);
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
