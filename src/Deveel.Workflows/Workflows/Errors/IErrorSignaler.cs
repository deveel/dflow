using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public interface IErrorSignaler
    {
        Task ThrowErrorAsync(ThrownError error, CancellationToken cancellationToken);
    }
}
