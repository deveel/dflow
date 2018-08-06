using System;

namespace Deveel.Workflows.Model
{
    public sealed class ManualTaskModel : TaskModel
    {
        internal override Activity BuildActivity(ModelBuildContext context)
        {
            return new ManualTask(Id);
        }
    }
}
