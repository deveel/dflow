using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public class EventNode : FlowNode
    {
        private readonly FlowEventHandler @event;

        public EventNode(string id, FlowEventHandler @event)
        : base(id)
        {
            this.@event = @event;
        }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var eventContext = @event.CreateContext(context);
            eventContext.Attach(async (c, s) => await ReactAsync(state, c));
        }

        protected virtual Task ReactAsync(object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
