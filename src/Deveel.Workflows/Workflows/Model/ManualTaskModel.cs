using System;

namespace Deveel.Workflows.Model
{
    public sealed class ManualTaskModel : TaskModel
    {
        internal override TaskBase BuildTask(ModelBuildContext context)
        {
            return new ManualTask(Id);
        }
    }
}
