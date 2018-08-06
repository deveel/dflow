using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Messaging
{
    public class MessageEventRise : IEventRise
    {
        private IMessagePublisher publisher;

        public MessageEventRise(IMessagePublisher publisher)
        {
            this.publisher = publisher;
        }

        EventType IEventRise.EventType => EventType.Message;

        public Task FireAsync(IEventArgument arg, CancellationToken cancellationToken)
        {
            var message = (Message)arg;
            return publisher.PusblishAsync(message, PublishBehavior.Notify, cancellationToken);
        }
    }
}
