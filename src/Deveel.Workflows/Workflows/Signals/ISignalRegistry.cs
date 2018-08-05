using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public interface ISignalRegistry
    {
        Task SubscribeAsync(string signal, ISignalHandler handler, CancellationToken cancellationToken);

        Task UnsubscribeAsync(string signal, string handlerId, CancellationToken cancellationToken);
    }
}
