using System;

namespace Deveel.Workflows.Events
{
    public interface IEventArgument
    {
        EventType EventType { get; }
    }
}
