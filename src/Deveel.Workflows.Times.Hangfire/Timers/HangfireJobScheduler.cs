using System;
using System.Threading;
using System.Threading.Tasks;

using Hangfire;
using Hangfire.Common;

namespace Deveel.Workflows.Timers
{
    public sealed class HangfireJobScheduler : IJobScheduler
    {
        private readonly BackgroundJobServer server;

        public HangfireJobScheduler()
        {
            
            server = new BackgroundJobServer();
        }

        public void Dispose()
        {
            server.Dispose();
        }

        public Task ScheduleAsync(string jobId, ScheduleInfo scheduleInfo, IScheduleCallback callback, CancellationToken cancellationToken)
        {
            if (scheduleInfo.Date != null)
            {
                BackgroundJob.Schedule(() => callback.NotifyAsync().Wait(), scheduleInfo.Date.Value);
            } else if (scheduleInfo.Duration != null) {
                BackgroundJob.Schedule(() => callback.NotifyAsync().Wait(), scheduleInfo.Duration.Value);
            } else {
                RecurringJob.AddOrUpdate(jobId, () => callback.NotifyAsync().Wait(), scheduleInfo.CronExpression);
            }

            return Task.CompletedTask;
        }

        public Task UnscheduleAsync(string jobId, CancellationToken cancellationToken)
        {
            RecurringJob.RemoveIfExists(jobId);
            BackgroundJob.Delete(jobId);
            return Task.CompletedTask;
        }
    }
}
