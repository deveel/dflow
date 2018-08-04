using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public interface IErrorHandler
    {
        Task ThrowErrorAsync(string processId, string instanceId, IError error, CancellationToken cancellationToken);
    }
}
