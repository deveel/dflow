using System;
using Deveel.Workflows.Events;

namespace Deveel.Workflows.Model
{
    public sealed class NoneEventModel : EventModel
    {
        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            return new Event(Id, new NoneEvent(Name));
        }
    }
}
