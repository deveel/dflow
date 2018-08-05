using System;
using Microsoft.Extensions.DependencyInjection;
using Deveel.Workflows.Errors;
using Xunit;

namespace Deveel.Workflows
{
    public class ErrorBoundaryEventsTests : TaskTestsBase
    {
        private InMemoryErrorSignal errorSignal;
        private bool errorCatched;

        protected override void AddServices(IServiceCollection services)
        {
            errorSignal = new InMemoryErrorSignal();

            services.AddSingleton<IErrorSignal>(errorSignal);
            services.AddSingleton<InMemoryErrorSignal>(errorSignal);
            services.AddSingleton<IErrorHandler, InMemoryErrorHandler>();
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            var errorEvent = new ErrorEvent(new ErrorEventSource(errorSignal), "err");

            sequence.Add(new ServiceTask("task1", c => throw new ErrorException("err"))
            {
                BoundaryEvents = {
                    new BoundaryEvent(errorEvent, new ServiceTask("catch", c => errorCatched = true))
                }
            });
        }

        [Fact]
        public async void ThrowAndCatchError()
        {
            await Process.RunAsync(Context);

            Assert.True(errorCatched);
        }
    }
}
