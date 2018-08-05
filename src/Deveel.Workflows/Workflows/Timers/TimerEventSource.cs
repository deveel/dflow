using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public sealed class TimerEventSource : IEventSource
    {
        private IJobScheduler scheduler;

        public TimerEventSource(IJobScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public EventType EventType => EventType.Timer;

        async Task IEventSource.AttachAsync(IEventContext context)
        {
            var timer = (TimerEvent)context.Event;
            var scheduleInfo = timer.ScheduleInfo;

            await scheduler.ScheduleAsync(context.EventId, scheduleInfo, new ScheduleCallback(context));
        }

        async Task IEventSource.DetachAsync(IEventContext context)
        {
            await scheduler.UnscheduleAsync(context.EventId);
        }

        public void Dispose()
        {
        }

        #region ScheduleCallback

        class ScheduleCallback : IScheduleCallback
        {
            private readonly IEventContext context;

            public ScheduleCallback(IEventContext context)
            {
                this.context = context;
            }

            public Task NotifyAsync()
            {
                return context.FireAsync();
            }
        }

        #endregion
    }
}
