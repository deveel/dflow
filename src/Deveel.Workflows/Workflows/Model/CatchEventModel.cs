using System;

namespace Deveel.Workflows.Model
{
    public sealed class CatchEventModel : EventModel
    {
        public EventSourceModel Event { get; }

        public string VariableName { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var source = Event.BuildSource(context);
            return new CatchEvent(Id, source, VariableName);
        }
    }
}
