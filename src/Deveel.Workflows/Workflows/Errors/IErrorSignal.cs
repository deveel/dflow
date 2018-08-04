using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public interface IErrorSignal
    {
        Task<ThrownError> WaitForErrorAsync(string errorName, CancellationToken cancellationToken);
    }
}
