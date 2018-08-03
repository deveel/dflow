using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegatedMergeStrategy : IMergeStrategy
    {
        private readonly Action<IExecutionContext> merge;

        public DelegatedMergeStrategy(Action<IExecutionContext> merge)
        {
            this.merge = merge ?? throw new ArgumentNullException(nameof(merge));
        }

        public Task MergeAsync(IExecutionContext context)
        {
            merge(context);
            return Task.CompletedTask;
        }
    }
}
