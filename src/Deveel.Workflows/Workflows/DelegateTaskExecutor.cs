using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegateTaskExecutor : ITaskExecutor
    {
        private readonly Action<ExecutionContext> execute;

        public DelegateTaskExecutor(Action<ExecutionContext> execute)
        {
            this.execute = execute;
        }

        public Task ExecuteAsync(ExecutionContext context)
        {
            execute(context);
            return Task.CompletedTask;
        }
    }
}
