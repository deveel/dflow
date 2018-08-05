using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public class EventNode : FlowNode
    {
        private readonly Event @event;

        public EventNode(string id, Event @event)
        : base(id)
        {
            this.@event = @event;
        }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var eventContext = await @event.CreateContextAsync(context);
            eventContext.Attach(async (e, c, s) => await ReactAsync(e, state, c));
        }

        protected virtual Task ReactAsync(Event source, object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
