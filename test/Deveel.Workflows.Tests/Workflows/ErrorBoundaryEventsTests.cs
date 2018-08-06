using System;
using Microsoft.Extensions.DependencyInjection;
using Deveel.Workflows.Errors;
using Xunit;
using System.Threading;

namespace Deveel.Workflows
{
    public class ErrorBoundaryEventsTests : TaskTestsBase
    {
        private InMemoryErrorHandler errorSignal;
        private AutoResetEvent catchEvent;

        public ErrorBoundaryEventsTests()
        {
            catchEvent = new AutoResetEvent(false);
        }

        protected override void AddServices(IServiceCollection services)
        {
            errorSignal = new InMemoryErrorHandler();

            services.AddSingleton<IErrorHandler>(errorSignal);
            services.AddSingleton<InMemoryErrorHandler>(errorSignal);
            services.AddSingleton<IErrorSignaler, InMemoryErrorSignaler>();
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            var errorEvent = new ErrorEventSource(errorSignal, "err");

            sequence.Add(new ServiceTask("task1", c => throw new ErrorException("err"))
            {
                BoundaryEvents = {
                    new BoundaryEvent(errorEvent, new ServiceTask("catch", c => catchEvent.Set()))
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
