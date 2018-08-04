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

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            @event.AttachToContext(context);
            @event.Attach(async (e, c) => await ReactAsync(e, state, c));
            return Task.CompletedTask;
        }

        protected virtual Task ReactAsync(IEvent source, object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
