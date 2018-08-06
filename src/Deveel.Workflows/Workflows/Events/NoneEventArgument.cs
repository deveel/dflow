using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEventArgument : IEventArgument
    {
        EventType IEventArgument.EventType => EventType.None;
    }
}
