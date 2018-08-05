using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Variables
{
    public interface IVariableHandler
    {
        Task NotifySetAsync(Variable variable, CancellationToken cancellationToken);
    }
}
