using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows
{
    public sealed class CatchEvent : FlowNode
    {
        private readonly EventSource source;

        public CatchEvent(string id, EventSource source, string variableName)
        : base(id)
        {
            this.source = source;
            VariableName = variableName;
        }

        public CatchEvent(string id, EventSource handler)
            : this(id, handler, null)
        {
        }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        public string VariableName { get; }

        protected override async Task ExecuteNodeAsync(object state, NodeContext context)
        {
            using (var eventContext = source.NewEventContext(context))
            {
                eventContext.Attach(async (c, s) => await ReactAsync(state, c));
                await eventContext.BeginAsync();

                // the event state is passed to the ReactAsync method
                eventContext.Wait(context.CancellationToken);
            }
        }

        private Task ReactAsync(object state, NodeContext context)
        {
            if (String.IsNullOrEmpty(VariableName))
                return Task.CompletedTask;

            // TODO: should we do anything different here?
            return context.Parent.SetVariableAsync(VariableName, state);
        }
    }
}
