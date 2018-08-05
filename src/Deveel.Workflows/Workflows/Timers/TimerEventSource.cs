using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public sealed class TimerEventSource : FlowEventSource
    {
        private IJobScheduler scheduler;

        public TimerEventSource(IJobScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        public override EventType EventType => EventType.Timer;

        protected override Task AttachContextAsync(EventContext context)
        {
            var timer = (TimerEvent)context.Event;
            var scheduleInfo = timer.ScheduleInfo;

            return scheduler.ScheduleAsync(context.EventId.ToString(), scheduleInfo, new ScheduleCallback(context), context.CancellationToken);
        }

        protected override Task DetachContextAsync(EventContext context)
        {
            return scheduler.UnscheduleAsync(context.EventId.ToString(), context.CancellationToken);
        }

        #region ScheduleCallback

        class ScheduleCallback : IScheduleCallback
        {
            private readonly EventContext context;

            public ScheduleCallback(EventContext context)
            {
                this.context = context;
            }

            public Task NotifyAsync()
            {
                return context.FireAsync(null);
            }
        }

        #endregion
    }
}
