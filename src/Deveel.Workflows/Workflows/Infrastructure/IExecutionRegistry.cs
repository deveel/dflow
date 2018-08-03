using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Infrastructure
{
    public interface IExecutionRegistry
    {
        Task RegisterAsync(string nodeId, IExecutionContext state);

        Task<IExecutionContext> FindStateAsync(string nodeId);
    }
}
