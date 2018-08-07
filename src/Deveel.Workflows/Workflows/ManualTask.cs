using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ManualTask : TaskBase
    {
        public ManualTask(string id) 
            : base(id)
        {
        }

        protected override Task ExecuteActivityAsync(ActivityContext context, object state)
        {
            return Task.CompletedTask;
        }
    }
}
