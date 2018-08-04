using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.States
{
    public interface IExecutionRegistry
    {
        Task RegisterAsync(ExecutionState state);

        Task<ExecutionState> FindStateAsync(string processId, string nodeId);
    }
}
