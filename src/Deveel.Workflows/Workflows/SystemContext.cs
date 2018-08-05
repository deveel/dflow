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

        public ProcessContext CreateContext(Process process, IActor actor, string instanceId)
        {
            return new ProcessContext(this, process, actor, instanceId);
        }

        public ProcessContext CreateContext(Process process, string instanceId)
            => CreateContext(process, new SystemUser(), instanceId);

        public ProcessContext CreateContext(Process process)
            => CreateContext(process, Guid.NewGuid().ToString());
    }
}
