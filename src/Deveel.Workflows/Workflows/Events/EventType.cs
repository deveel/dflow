using System;

namespace Deveel.Workflows.Events
{
    public enum EventType
    {
        None,
        Timer,
        Message,
        Signal,
        Error,
        Escalation
    }
}
