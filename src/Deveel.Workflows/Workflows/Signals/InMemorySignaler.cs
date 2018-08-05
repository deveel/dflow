using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public sealed class InMemorySignaler : ISignaler
    {
        private readonly InMemorySignalRegistry registry;

        public InMemorySignaler(InMemorySignalRegistry registry)
        {
            this.registry = registry;
        }

        public Task SignalAsync(Signal signal, CancellationToken cancellationToken)
        {
            return registry.SignalAsync(signal, cancellationToken);
        }
    }
}
