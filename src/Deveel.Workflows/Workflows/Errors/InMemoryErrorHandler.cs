using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class InMemoryErrorHandler : IErrorHandler
    {
        private readonly InMemoryErrorSignal signal;

        public InMemoryErrorHandler(InMemoryErrorSignal signal)
        {
            this.signal = signal;
        }

        public Task ThrowErrorAsync(string processId, string instanceId, IError error, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            signal.OnErrorThrown(new ThrownError(processId, instanceId, error.Name));
            return Task.CompletedTask;
        }
    }
}
