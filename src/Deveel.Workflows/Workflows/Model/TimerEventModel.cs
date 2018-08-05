using Deveel.Workflows.Timers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Deveel.Workflows.Model
{
    public sealed class TimerEventModel : EventModel
    {
        public string CronExpression { get; set; }

        public string Duration { get; set; }

        public string Date { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var scheduler = context.Context.GetRequiredService<IJobScheduler>();
            var source = new TimerEventSource(scheduler);
            var scheduleInfo = new ScheduleInfo
            {
                CronExpression = CronExpression,
                Duration = String.IsNullOrEmpty(Duration) ? (TimeSpan?) null : TimeSpan.Parse(Duration),
                Date = String.IsNullOrEmpty(Date) ? (DateTimeOffset?)null : DateTimeOffset.Parse(Date)
            };

            return new EventNode(Id, new TimerEvent(source, Name, scheduleInfo));
        }
    }
}
