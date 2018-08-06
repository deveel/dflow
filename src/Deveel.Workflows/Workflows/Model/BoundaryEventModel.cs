using System;

namespace Deveel.Workflows.Model
{
    public class BoundaryEventModel : FlowNodeModel
    {
        public EventSourceModel EventSource { get; set; }

        public string EventName { get; set; }

        public FlowNode Outgoing { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var compiledEvent = EventSource.BuildSource(context, EventName);

            return new BoundaryEvent(Id, compiledEvent, Outgoing);
        }
    }
}
