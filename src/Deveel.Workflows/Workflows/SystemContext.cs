using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;
using System;
using System.Threading;

namespace Deveel.Workflows
{
    public sealed class SystemContext : IContext
    {
        private IServiceProvider provider;

        public SystemContext(IServiceProvider provider)
        {
            this.provider = provider;
        }

        IContext IContext.Parent => null;

        CancellationToken IContext.CancellationToken => CancellationToken.None;

        public void Dispose()
        {
            provider = null;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return provider.GetService(serviceType);
        }

        public ProcessContext CreateContext(Process process, IActor actor, EventSource trigger)
        {
            return new ProcessContext(this, process, actor, trigger);
        }

        public ProcessContext CreateContext(Process process, IActor actor)
            => CreateContext(process, actor, NoneEventSource.Instance);

        public ProcessContext CreateContext(Process process, EventSource trigger)
            => CreateContext(process, SystemUser.Current, trigger);

        public ProcessContext CreateContext(Process process)
            => CreateContext(process, SystemUser.Current);
    }
}
