using Deveel.Workflows.Expressions;
using System;

namespace Deveel.Workflows.Model
{
    public abstract class ActivityModel : FlowNodeModel
    {
        public string LoopCondition { get; set; }

        internal abstract Activity BuildActivity(ModelBuildContext buildContext);

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var task = BuildActivity(context);

            if (!String.IsNullOrWhiteSpace(LoopCondition))
                return new ActivityLoop(task, FlowExpression.Parse(LoopCondition));

            // TODO: multi-instances

            // TODO: boundary events

            return task;
        }
    }
}
