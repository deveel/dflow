using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public sealed class TimerEventSource : EventSource
    {
        private IJobScheduler scheduler;

        public TimerEventSource(IJobScheduler scheduler, string eventName, ScheduleInfo scheduleInfo)
            : base(eventName)
        {
            this.scheduler = scheduler;
            ScheduleInfo = scheduleInfo;
        }

        public ScheduleInfo ScheduleInfo { get; }

        public override EventType EventType => EventType.Timer;

        protected override Task AttachContextAsync(EventContext context)
        {
            return scheduler.ScheduleAsync(context.EventId.ToString(), ScheduleInfo, new ScheduleCallback(context), context.CancellationToken);
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
