using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public sealed class InMemorySignalRegistry : ISignalRegistry, ISignaler
    {
        private readonly Dictionary<string, IDictionary<string, ISignalHandler>> handlers;

        public Task SignalAsync(Signal signal, CancellationToken cancellationToken)
        {
            IDictionary<string, ISignalHandler> signalHandlers = null;
            if (handlers.TryGetValue(signal.Name, out signalHandlers))
            {
                var tasks = signalHandlers.Values.Select(x => x.HandleAsync(signal, cancellationToken));
                return Task.WhenAll(tasks);
            }

            return Task.CompletedTask;
        }

        public Task SubscribeAsync(string signal, ISignalHandler handler, CancellationToken cancellationToken)
        {
            IDictionary<string, ISignalHandler> signalHandlers = null;
            if (!handlers.TryGetValue(signal, out signalHandlers))
            {
                handlers[signal] = signalHandlers = new Dictionary<string, ISignalHandler>();
            }

            signalHandlers[handler.Id] = handler;

            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(string signal, string handlerId, CancellationToken cancellationToken)
        {
            IDictionary<string, ISignalHandler> signalHandlers = null;
            if (handlers.TryGetValue(signal, out signalHandlers))
            {
                signalHandlers.Remove(handlerId);
            }

            return Task.CompletedTask;
        }
    }
}
