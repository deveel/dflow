using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEventSource : EventSource
    {
        public NoneEventSource(string eventName) : base(eventName)
        {
        }

        public override EventType EventType => EventType.None;
    }
}
