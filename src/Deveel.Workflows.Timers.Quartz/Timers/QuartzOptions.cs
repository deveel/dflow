using System;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Timers
{
    public sealed class QuartzOptions
    {
        public QuartzOptions()
        {
            SchedulerName = "Workflows Quartz.Net Scheduler";
        }

        public string SchedulerName { get; set; }
    }
}
