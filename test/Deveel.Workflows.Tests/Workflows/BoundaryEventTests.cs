using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Events;
using Deveel.Workflows.Timers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Deveel.Workflows
{
    public class BoundaryEventTests : TaskTestsBase
    {
        private bool timeout;

        protected override void AddServices(IServiceCollection services)
        {
            services.AddHangfireScheduler();
        }

        protected override void OnTaskAdd(ProcessSequence sequence)
        {
            var scheduler = SystemContext.GetRequiredService<IJobScheduler>();

            sequence.Add(new ServiceTask("waitTask", context => Thread.Sleep(2000))
            {
                BoundaryEvents =
               {
                   new BoundaryEvent("b1", new TimerEventSource(scheduler, "timer1", new ScheduleInfo
                   {

                       Duration = TimeSpan.FromMilliseconds(200)
                   }), new ServiceTask("timeoutTask", context => timeout = true))
               }
            });
        }

        [Fact]
        public async void TimeoutTask()
        {
            await Process.RunAsync(Context);

            Assert.True(timeout);
        }
    }
}
