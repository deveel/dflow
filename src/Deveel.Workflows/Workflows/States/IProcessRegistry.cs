using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.States
{
    public interface IProcessRegistry
    {
        Task RegisterAsync(ProcessState state, CancellationToken cancellationToken);
    }
}
