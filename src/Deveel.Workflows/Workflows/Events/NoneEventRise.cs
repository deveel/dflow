using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEventRise : IEventRise
    {
        EventType IEventRise.EventType => EventType.None;

        Task IEventRise.FireAsync(IEventArgument arg, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
