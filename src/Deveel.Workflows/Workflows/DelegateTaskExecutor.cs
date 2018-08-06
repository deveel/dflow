using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegateTaskExecutor : ITaskExecutor
    {
        private readonly Action<NodeContext> execute;

        public DelegateTaskExecutor(Action<NodeContext> execute)
        {
            this.execute = execute;
        }

        public Task ExecuteAsync(NodeContext context)
        {
            execute(context);
            return Task.CompletedTask;
        }
    }
}
