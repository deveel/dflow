using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public static class JobSchedulerExtensions
    {
        public static Task ScheduleAsync(this IJobScheduler scheduler, string jobId, ScheduleInfo scheduleInfo, Action notify, CancellationToken cancellationToken = default(CancellationToken))
            => scheduler.ScheduleAsync(jobId, scheduleInfo, new ActionCallback(notify), cancellationToken);

        #region Callback

        class ActionCallback : IScheduleCallback
        {
            private readonly Action action;

            public ActionCallback(Action action)
            {
                this.action = action;
            }

            public Task NotifyAsync()
            {
                return Task.Run(action);
            }
        }

        #endregion
    }
}
