using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public interface IEventRise
    {
        EventType EventType { get; }


        Task FireAsync(IEventArgument arg, CancellationToken cancellationToken);
    }
}
