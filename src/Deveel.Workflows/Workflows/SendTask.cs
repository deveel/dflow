using Deveel.Workflows.Messaging;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class SendTask : TaskBase
    {
        private readonly IMessagePublisher publisher;

        public SendTask(string id, IMessagePublisher publisher, Message message) : base(id)
        {
            this.publisher = publisher;
            Message = message;
        }

        public Message Message { get; }

        protected override Task ExecuteActivityAsync(ActivityContext context, object state)
        {
            // TODO: support for sagas?
            return publisher.PusblishAsync(Message, PublishBehavior.Notify, context.CancellationToken);
        }
    }
}
