using System;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(IActor actor, IServiceScope provider) 
            : this(new ManualEvent(), actor, provider)
        {
        }

        public ExecutionContext(IEvent trigger, IActor actor, IServiceScope provider)
        {
            Actor = actor;
            Trigger = trigger;
            Provider = provider;
        }

        public IServiceScope Provider { get; }

        public IActor Actor { get; }

        public IEvent Trigger { get; }

        public IExecutionContext CreateScope()
        {
            return new ExecutionContext(Actor, Provider.ServiceProvider.CreateScope());
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return Provider == null ? null : Provider.ServiceProvider.GetService(serviceType);
        }
    }
}
