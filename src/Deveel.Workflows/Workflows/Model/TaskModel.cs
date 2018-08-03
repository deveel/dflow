using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public abstract class TaskModel : ActivityModel
    {
        internal abstract TaskBase BuildTask(ModelBuildContext context);

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var task = BuildTask(context);

            if (!String.IsNullOrWhiteSpace(LoopCondition))
                return new ActivityLoop(task, FlowExpression.Parse(LoopCondition));

            return task;
        }
    }
}
