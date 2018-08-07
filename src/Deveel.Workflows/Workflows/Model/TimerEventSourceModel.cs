using System;
using Deveel.Workflows.Events;
using Deveel.Workflows.Timers;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class TimerEventSourceModel : EventSourceModel
    {
        public string CronExpression { get; set; }

        public string Duration { get; set; }

        public string Date { get; set; }

        internal override EventSource BuildSource(ModelBuildContext context)
        {
            var scheduler = context.Context.GetRequiredService<IJobScheduler>();
            var scheduleInfo = new ScheduleInfo
            {
                CronExpression = CronExpression,
                Duration = String.IsNullOrEmpty(Duration) ? (TimeSpan?)null : TimeSpan.Parse(Duration),
                Date = String.IsNullOrEmpty(Date) ? (DateTimeOffset?)null : DateTimeOffset.Parse(Date)
            };

            return new TimerEventSource(scheduler, EventName, scheduleInfo);
        }
    }
}
