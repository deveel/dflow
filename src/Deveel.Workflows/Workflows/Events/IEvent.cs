using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public interface IEvent : IDisposable
    {
        string Name { get; }

        IEventSource Source { get; }

        Task<IEventContext> CreateContextAsync(ExecutionContext context);
    }
}
