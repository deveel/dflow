using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public interface IErrorHandler
    {
        Task<ThrownError> CatchErrorAsync(string processId, string instanceId, string errorCode, CancellationToken cancellationToken);
    }
}
