using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegatedMergeStrategy : IMergeStrategy
    {
        private readonly Action<NodeContext> merge;

        public DelegatedMergeStrategy(Action<NodeContext> merge)
        {
            this.merge = merge ?? throw new ArgumentNullException(nameof(merge));
        }

        public Task MergeAsync(NodeContext context)
        {
            merge(context);
            return Task.CompletedTask;
        }
    }
}
