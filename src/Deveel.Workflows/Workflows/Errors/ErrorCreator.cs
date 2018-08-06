using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorCreator : IEventRise
    {
        private readonly IErrorSignaler signaler;

        public ErrorCreator(IErrorSignaler signaler)
        {
            this.signaler = signaler;
        }

        EventType IEventRise.EventType => EventType.Error;

        public Task FireAsync(IEventArgument arg, CancellationToken cancellationToken)
        {
            var error = (ThrownError)arg;
            return signaler.ThrowErrorAsync(error, cancellationToken);
        }
    }
}
