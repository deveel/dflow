using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class InMemoryErrorSignaler : IErrorSignaler
    {
        private readonly InMemoryErrorHandler handler;

        public InMemoryErrorSignaler(InMemoryErrorHandler handler)
        {
            this.handler = handler;
        }

        public Task ThrowErrorAsync(ThrownError error, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ((IErrorSignalHandler)handler).SignalAsync(error, cancellationToken);
        }
    }
}
