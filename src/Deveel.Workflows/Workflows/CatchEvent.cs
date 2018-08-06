using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows
{
    public sealed class CatchEvent : FlowNode
    {
        private readonly FlowEventHandler handler;

        public CatchEvent(string id, FlowEventHandler handler, string variableName)
        : base(id)
        {
            this.handler = handler;
            VariableName = variableName;
        }

        public CatchEvent(string id, FlowEventHandler handler)
            : this(id, handler, null)
        {
        }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        public string VariableName { get; }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            using (var eventContext = handler.CreateContext(context))
            {
                eventContext.Attach(async (c, s) => await ReactAsync(state, c));
                await eventContext.BeginAsync();

                // the event state is passed to the ReactAsync method
                eventContext.Wait(context.CancellationToken);
            }
        }

        private Task ReactAsync(object state, ExecutionContext context)
        {
            if (String.IsNullOrEmpty(VariableName))
                return Task.CompletedTask;

            // TODO: should we do anything different here?
            return context.Parent.SetVariableAsync(VariableName, state);
        }
    }
}
