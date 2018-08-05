using Deveel.Workflows.Events;
using Deveel.Workflows.Messaging;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ReceiveTask : TaskBase
    {
        public ReceiveTask(string id, MessageEvent eventSource)
            : base(id)
        {
            Event = eventSource;
        }

        public MessageEvent Event { get; }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            using (var eventContext = await Event.CreateContextAsync(context))
            {
                var message = eventContext.Wait(context.CancellationToken);

                // TODO: what to do with the received message?
            }
        }
    }
}