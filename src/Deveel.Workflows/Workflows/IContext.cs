using System;

namespace Deveel.Workflows
{
    public interface IContext : IServiceProvider, IDisposable
    {
        IContext Parent { get; }
    }
}
