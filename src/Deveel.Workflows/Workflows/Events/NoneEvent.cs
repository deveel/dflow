using System;
using System.Threading.Tasks;

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

        Task<IEventContext> IEvent.CreateContextAsync(ExecutionContext context)
        {
            return Task.FromResult<IEventContext>(new EventContext<NoneEvent>(this, context));
        }

        #region NoneEventSource

        class NoneEventSource : IEventSource
        {
            public EventType EventType => EventType.None;

            public Task AttachAsync(IEventContext context)
            {
                return Task.CompletedTask;
            }

            public Task DetachAsync(IEventContext context)
            {
                return Task.CompletedTask;
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
