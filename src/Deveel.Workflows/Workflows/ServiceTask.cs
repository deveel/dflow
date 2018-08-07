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

        public ServiceTask(string id, Action<NodeContext> execute)
            : this(id, new DelegateTaskExecutor(execute))
        {
        }

        public ITaskExecutor Executor { get; }

        protected override Task ExecuteActivityAsync(ActivityContext context, object state)
        {
            return Executor.ExecuteAsync(context);
        }
    }
}
