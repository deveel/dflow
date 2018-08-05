using System;

namespace Deveel.Workflows.Timers
{
    public sealed class ScheduleInfo
    {

        public string CronExpression { get; set; }

        public TimeSpan? Duration { get; set; }

        public DateTimeOffset? Date { get; set; }
    }
}
