using Deveel.Workflows.Events;
using Deveel.Workflows.Messaging;
using Deveel.Workflows.Variables;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ReceiveTask : TaskBase
    {
        public ReceiveTask(string id, MessageEventSource source, string variableName)
            : base(id)
        {
            EventSource = source;
            MessageVariableName = variableName;
        }

        public MessageEventSource EventSource { get; }

        public string MessageVariableName { get; }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            using (var eventContext = EventSource.NewEventContext(context))
            {
                var message = eventContext.Wait(context.CancellationToken);

                await context.Parent.SetVariableAsync(MessageVariableName, message);
            }
        }
    }
}