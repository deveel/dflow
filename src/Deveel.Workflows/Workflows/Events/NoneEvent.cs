using System;

namespace Deveel.Workflows.Events
{
    public sealed class NoneEvent : IEvent
    {
        public NoneEvent() : this("none")
        {
        }

        public NoneEvent(string name)
        {
            Name = name;
        }

        public string Name { get; }

        IEventSource IEvent.Source => new NoneEventSource();

        void IEvent.AttachToContext(ExecutionContext context)
        {
        }

        void IEvent.Attach(Action<IEvent, ExecutionContext> callback)
        {
        }

        void IEvent.Detach(Action<IEvent, ExecutionContext> callback)
        {
        }

        #region NoneEventSource

        class NoneEventSource : IEventSource
        {
            public EventType EventType => EventType.None;

            public void Attach(IEvent @event)
            {
            }

            public void Dispose()
            {
            }
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
