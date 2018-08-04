using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegatedMergeStrategy : IMergeStrategy
    {
        private readonly Action<ExecutionContext> merge;

        public DelegatedMergeStrategy(Action<ExecutionContext> merge)
        {
            this.merge = merge ?? throw new ArgumentNullException(nameof(merge));
        }

        public Task MergeAsync(ExecutionContext context)
        {
            merge(context);
            return Task.CompletedTask;
        }
    }
}
