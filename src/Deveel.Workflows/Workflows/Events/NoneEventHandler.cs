using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEvent : FlowEventHandler
    {
        public NoneEvent() : this("none")
        {
        }

        public NoneEvent(string name)
            : base(new NoneEventSource(), name)
        {
        }

        #region NoneEventSource

        class NoneEventSource : FlowEventSource
        {
            public override EventType EventType => EventType.None;
        }

        #endregion
    }
}
