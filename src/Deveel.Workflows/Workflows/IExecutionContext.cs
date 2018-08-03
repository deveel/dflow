using System;
using Deveel.Workflows.Actors;

namespace Deveel.Workflows
{
    public interface IExecutionContext : IContext
    {
        IActor Actor { get; }

        IEvent Trigger { get; }

        IExecutionContext CreateScope();
    }
}
