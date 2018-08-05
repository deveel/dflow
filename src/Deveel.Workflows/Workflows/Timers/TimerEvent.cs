using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows.Timers
{
    public sealed class TimerEvent : Event
    {
        public TimerEvent(TimerEventSource eventSource, string name, ScheduleInfo scheduleInfo)
            : base(eventSource, name) {
            ScheduleInfo = scheduleInfo;
        }

        public ScheduleInfo ScheduleInfo { get; }
    }
}
