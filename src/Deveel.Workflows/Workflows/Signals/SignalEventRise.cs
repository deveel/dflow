using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public sealed class SignalEventRise : IEventRise
    {
        private readonly ISignaler signaler;

        public SignalEventRise(ISignaler signaler)
        {
            this.signaler = signaler;
        }

        EventType IEventRise.EventType => EventType.Signal;

        Task IEventRise.FireAsync(IEventArgument arg, CancellationToken cancellationToken)
        {
            return FireAsync((Signal) arg, cancellationToken);
        }

        public Task FireAsync(Signal signal, CancellationToken cancellationToken)
        {
            return signaler.SignalAsync(signal, cancellationToken);
        }
    }
}
