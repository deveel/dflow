using System;
using System.Threading;

namespace Deveel.Workflows
{
    public interface IContext : IServiceProvider, IDisposable
    {
        IContext Parent { get; }

        CancellationToken CancellationToken { get; }
    }
}
