using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows
{
    public class Event : FlowNode
    {
        private readonly IEvent @event;

        public Event(string id, IEvent @event)
        : base(id)
        {
            this.@event = @event;
        }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var eventContext = await @event.CreateContextAsync(context);
            eventContext.Attach(async (e, c) => await ReactAsync(e, state, c));
        }

        protected virtual Task ReactAsync(IEvent source, object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
