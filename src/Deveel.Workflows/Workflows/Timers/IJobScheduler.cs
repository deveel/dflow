using Deveel.Workflows.Events;
using System;

using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public interface IJobScheduler : IDisposable
    {
        Task ScheduleAsync(EventId eventId, ScheduleInfo scheduleInfo, IScheduleCallback callback);

        Task UnscheduleAsync(EventId eventId);
    }
}
