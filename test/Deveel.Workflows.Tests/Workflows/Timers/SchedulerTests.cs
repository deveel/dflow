using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Xunit;

namespace Deveel.Workflows.Timers
{
    public class SchedulerTests
    {
        private IJobScheduler scheduler;

        public SchedulerTests()
        {
            CreateScheduler();
        }

        private void CreateScheduler()
        {
            var provider = new ServiceCollection()
                .AddHangfireScheduler()
                .BuildServiceProvider();

            scheduler = provider.GetRequiredService<IJobScheduler>();
        }

        [Fact]
        public async void ScheduleJob()
        {
            var executed = new AutoResetEvent(false);

            await scheduler.ScheduleAsync("job1", new ScheduleInfo() { Duration=TimeSpan.FromMilliseconds(500) }, () => executed.Set());

            executed.WaitOne();

        }
    }
}
