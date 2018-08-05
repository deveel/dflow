using System;
using System.Threading;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ProcessContext : IContext
    {
        private IServiceScope scope;

        public ProcessContext(SystemContext parent, ProcessInfo processInfo, IActor actor, IEvent trigger, string instanceId)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            ProcessInfo = processInfo;
            Actor = actor;
            Trigger = trigger;
            scope = parent.CreateScope();
            InstanceId = instanceId;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        public IContext Parent { get; }

        public string InstanceId { get; }

        public ProcessInfo ProcessInfo { get; }

        public string Id => ProcessInfo.Id;

        public IActor Actor { get; }

        public IEvent Trigger { get; }

        public ExecutionContext CreateContext(FlowNode node)
        {
            return new ExecutionContext(this, node);
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
