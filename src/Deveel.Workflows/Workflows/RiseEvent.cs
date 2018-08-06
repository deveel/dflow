using Deveel.Workflows.Events;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class RiseEvent : FlowNode
    {
        private readonly IEventRise creator;

        public RiseEvent(string id, IEventRise creator, IEventArgument arg) : base(id)
        {
            this.creator = creator;
            Argument = arg;
        }

        public IEventArgument Argument { get; }

        public override FlowNodeType NodeType => FlowNodeType.Event;

        public EventType EventType => creator.EventType;

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            await creator.FireAsync(Argument, context.CancellationToken);
        }
    }
}
