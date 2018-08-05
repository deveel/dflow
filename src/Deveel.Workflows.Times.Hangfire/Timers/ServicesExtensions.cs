using Microsoft.Extensions.DependencyInjection;
using System;

using Hangfire;
using Hangfire.MemoryStorage;

namespace Deveel.Workflows.Timers
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddHangfireScheduler(this IServiceCollection services)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            GlobalConfiguration.Configuration.UseDefaultActivator();

            services.AddSingleton<IJobScheduler, HangfireJobScheduler>();

            return services;
        }

        class ServiceJobActivator : JobActivator
        {
            private IServiceProvider provider;

            public override object ActivateJob(Type jobType)
            {
                return provider.GetService(jobType);
            }
        }
    }
}
