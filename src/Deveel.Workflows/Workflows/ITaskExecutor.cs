using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public interface ITaskExecutor
    {
        Task ExecuteAsync(IExecutionContext context);
    }
}
