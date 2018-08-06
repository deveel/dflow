using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEventSource : EventSource
    {
        public NoneEventSource(string eventName) : base(eventName)
        {
        }

        public NoneEventSource()
            : this(null)
        {
        }

        static NoneEventSource()
        {
            Instance = new NoneEventSource();
        }

        public override EventType EventType => EventType.None;

        public static NoneEventSource Instance { get; }
    }
}
