using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEventHandler : FlowEventHandler
    {
        public NoneEventHandler() : this("none")
        {
        }

        public NoneEventHandler(string name)
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
