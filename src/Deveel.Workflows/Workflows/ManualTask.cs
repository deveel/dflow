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

        internal override Task ExecuteNodeAsync(IExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
