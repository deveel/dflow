using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class DelegateTaskExecutor : ITaskExecutor
    {
        private readonly Action<IExecutionContext> execute;

        public DelegateTaskExecutor(Action<IExecutionContext> execute)
        {
            this.execute = execute;
        }

        public Task ExecuteAsync(IExecutionContext context)
        {
            execute(context);
            return Task.CompletedTask;
        }
    }
}
