using System;
using Microsoft.Extensions.DependencyInjection;
using Deveel.Workflows.Errors;
using Xunit;
using System.Threading;

namespace Deveel.Workflows
{
    public class ErrorBoundaryEventsTests : TaskTestsBase
    {
        private InMemoryErrorHandler errorHandler;
        private AutoResetEvent catchEvent;

        public ErrorBoundaryEventsTests()
        {
            catchEvent = new AutoResetEvent(false);
        }

        protected override void AddServices(IServiceCollection services)
        {
            errorHandler = new InMemoryErrorHandler();

            services.AddSingleton<IErrorHandler>(errorHandler);
            services.AddSingleton<InMemoryErrorHandler>(errorHandler);
            services.AddSingleton<IErrorSignaler, InMemoryErrorSignaler>();
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            var errorEvent = new ErrorEventSource(errorHandler, "error", "ERR", "ERR-022");

            sequence.Add(new ServiceTask("task1", c => throw new ErrorException("ERR", "ERR-022"))
            {
                BoundaryEvents = {
                    new BoundaryEvent("b1", errorEvent, new ServiceTask("catch", c => catchEvent.Set()))
                }
            });
        }

        [Fact]
        public async void ThrowAndCatchError()
        {
            await Process.RunAsync(Context);

            catchEvent.WaitOne();
        }
    }
}
