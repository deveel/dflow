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

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
