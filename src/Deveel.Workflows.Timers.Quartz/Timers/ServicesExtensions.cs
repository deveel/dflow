using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;

namespace Deveel.Workflows.Timers
{
    public static class ServicesExtensions
    {

        public static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
        {
            return services.AddSingleton<IJobScheduler, QuartzJobScheduler>(p =>
            {
                QuartzNetScheduler.Initialize(new QuartzOptions());

                return new QuartzJobScheduler(QuartzNetScheduler.Scheduler);
            });
        }
    }
}