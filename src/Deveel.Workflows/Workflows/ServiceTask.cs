using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ServiceTask : TaskBase
    {
        public ServiceTask(string id, ITaskExecutor executor)
            : base(id)
        {
            Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public ServiceTask(string id, Action<IExecutionContext> execute)
            : this(id, new DelegateTaskExecutor(execute))
        {
        }

        public ITaskExecutor Executor { get; }

        internal override Task ExecuteNodeAsync(IExecutionContext context)
        {
            return Executor.ExecuteAsync(context);
        }
    }
}
