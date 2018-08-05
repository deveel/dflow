using Quartz;
using Quartz.Impl;
using System;

namespace Deveel.Workflows.Timers
{
    public static class QuartzNetScheduler
    {
        public static void Initialize(QuartzOptions options)
        {
            if (Scheduler == null)
            {
                var factory = new StdSchedulerFactory();
                Scheduler = factory.GetScheduler().Result;
            }
        }

        internal static IScheduler Scheduler { get; private set; }
    }
}
