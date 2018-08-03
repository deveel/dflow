using System;

namespace Deveel.Workflows.Model
{
    public sealed class NoneEventModel : EventModel
    {
        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            return new NoneEvent(Id, Name);
        }
    }
}
