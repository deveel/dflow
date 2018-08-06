using Deveel.Workflows.Events;
using Deveel.Workflows.Messaging;
using Deveel.Workflows.Variables;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ReceiveTask : TaskBase
    {
        public ReceiveTask(string id, MessageEventHandler handler, string variableName)
            : base(id)
        {
            Event = handler;
            MessageVariableName = variableName;
        }

        public MessageEventHandler Event { get; }

        public string MessageVariableName { get; }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            using (var eventContext = Event.CreateContext(context))
            {
                var message = eventContext.Wait(context.CancellationToken);

                await context.Parent.SetVariableAsync(MessageVariableName, message);
            }
        }
    }
}