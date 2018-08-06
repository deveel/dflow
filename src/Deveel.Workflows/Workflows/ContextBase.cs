using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Deveel.Workflows
{
    public abstract class ContextBase : IContext
    {
        private IServiceScope scope;

        protected ContextBase(IContext parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            scope = parent.CreateScope();
            CancellationToken = parent.CancellationToken;
            ParentContext = parent;
        }

        ~ContextBase()
        {
            Dispose(false);
        }

        IContext IContext.Parent => ParentContext;

        protected IContext ParentContext { get; }

        public virtual CancellationToken CancellationToken { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                scope?.Dispose();
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }
    }
}
