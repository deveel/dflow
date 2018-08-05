using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;

namespace Deveel.Workflows.Timers
{
    public static class ServicesExtensions
    {
        public static void AddQuartzScheduler(this IServiceCollection services)
        {
            services.AddSingleton<ISchedulerFactory>(p => new StdSchedulerFactory());

            services.AddSingleton<IJobScheduler, QuartzJobScheduler>(p =>
            {
                var factory = p.GetRequiredService<ISchedulerFactory>();
                var scheduler = factory.GetScheduler().Result;
                return new QuartzJobScheduler(scheduler);
            });
        }
    }
}
