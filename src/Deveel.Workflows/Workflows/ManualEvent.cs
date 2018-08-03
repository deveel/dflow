using System;

namespace Deveel.Workflows
{
    public sealed class ManualEvent : IEvent
    {
        string IEvent.Name => "manual";
    }
}
