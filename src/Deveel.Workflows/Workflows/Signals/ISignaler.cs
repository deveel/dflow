using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public interface ISignaler
    {
        Task SignalAsync(Signal signal, CancellationToken cancellationToken);
    }
}
