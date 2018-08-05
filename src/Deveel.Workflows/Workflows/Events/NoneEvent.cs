using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEvent : Event
    {
        public NoneEvent() : this("none")
        {
        }

        public NoneEvent(string name)
            : base(new NoneEventSource(), name)
        {
        }

        #region NoneEventSource

        class NoneEventSource : EventSource
        {
            public override EventType EventType => EventType.None;
        }

        #endregion
    }
}
