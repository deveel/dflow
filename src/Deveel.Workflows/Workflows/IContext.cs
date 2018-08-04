using System;

namespace Deveel.Workflows
{
    public interface IContext : IServiceProvider
    {
        IContext Parent { get; }
    }
}
