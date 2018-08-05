using System;
using System.Collections.Generic;
using System.Threading;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptContext : IContext
    {
        private IServiceScope scope;

        internal ScriptContext(ExecutionContext context)
        {
            Parent = context;
            scope = context.CreateScope();
        }

        public ExecutionContext Parent { get; }

        IContext IContext.Parent => Parent;

        CancellationToken IContext.CancellationToken => CancellationToken.None;

        object IServiceProvider.GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
