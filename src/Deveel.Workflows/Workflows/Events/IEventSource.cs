using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public interface IEventSource : IDisposable
    {
        EventType EventType { get; }

        Task AttachAsync(IEventContext context);

        Task DetachAsync(IEventContext context);
    }
}
