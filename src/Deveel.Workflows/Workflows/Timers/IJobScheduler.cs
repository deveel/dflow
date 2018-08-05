using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public interface IJobScheduler : IDisposable
    {
        Task ScheduleAsync(string jobId, ScheduleInfo scheduleInfo, IScheduleCallback callback, CancellationToken cancellationToken);

        Task UnscheduleAsync(string jobId, CancellationToken cancellationToken);
    }
}
