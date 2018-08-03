using System;

namespace Deveel.Workflows.Model
{
    public abstract class ActivityModel : FlowNodeModel
    {
        public string LoopCondition { get; set; }

    }
}
