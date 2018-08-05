using System;
using System.Threading;
using Xunit;

namespace Deveel.Workflows.Errors
{
    public class InMemoryErrorCatchTests
    {
        private InMemoryErrorSignaler signal;
        private InMemoryErrorHandler handler;

        public InMemoryErrorCatchTests()
        {
            handler = new InMemoryErrorHandler();
            signal = new InMemoryErrorSignaler(handler);
        }

        [Fact]
        public async void ThrowAndCatch()
        {
            const string processId = "proc1";
            var instanceId = Guid.NewGuid().ToString();
            var error = new ThrownError(processId, instanceId, "error");

            await signal.ThrowErrorAsync(error, CancellationToken.None);

            var thrownError = await handler.CatchErrorAsync(processId, instanceId, "error", CancellationToken.None);

            Assert.NotNull(thrownError);
        }
    }
}
