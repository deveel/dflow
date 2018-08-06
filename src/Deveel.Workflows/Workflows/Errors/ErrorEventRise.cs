using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventRise : IEventRise
    {
        private readonly IErrorSignaler signaler;

        public ErrorEventRise(IErrorSignaler signaler)
        {
            this.signaler = signaler;
        }

        EventType IEventRise.EventType => EventType.Error;

        Task IEventRise.FireAsync(IEventArgument arg, CancellationToken cancellationToken)
        {
            return FireAsync((ThrownError)arg, cancellationToken);
        }

        public Task FireAsync(ThrownError error, CancellationToken cancellationToken)
        {
            return signaler.ThrowErrorAsync(error, cancellationToken);
        }
    }
}
