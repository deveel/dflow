using Deveel.Workflows.Events;
using Deveel.Workflows.Messaging;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ReceiveTask : TaskBase
    {
        public ReceiveTask(string id, MessageEventHandler eventSource)
            : base(id)
        {
            Event = eventSource;
        }

        public MessageEventHandler Event { get; }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            using (var eventContext = Event.CreateContext(context))
            {
                var message = eventContext.Wait(context.CancellationToken);

                // TODO: what to do with the received message?
            }
        }
    }
}