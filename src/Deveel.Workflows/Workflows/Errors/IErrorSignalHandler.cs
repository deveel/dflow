using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public interface IErrorSignalHandler : IErrorHandler
    {
        Task SignalAsync(ThrownError error, CancellationToken cancellationToken);
    }
}
