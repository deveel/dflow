using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public interface IEventContext : IContext
    {
        IEvent Event { get; }

        EventId EventId { get; }

        void Attach(Action<IEvent, ExecutionContext> callback);

        void Detach(Action<IEvent, ExecutionContext> callback);

        Task FireAsync();
    }
}
