using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public interface ISignalHandler
    {
        string Id { get; }

        Task HandleAsync(Signal signal, CancellationToken cancellationToken);
    }
}
