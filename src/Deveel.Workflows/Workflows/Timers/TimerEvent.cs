using System;
using System.Threading.Tasks;
using Deveel.Workflows.Events;

namespace Deveel.Workflows.Timers
{
    public sealed class TimerEvent : IEvent
    {
        private TimerEventSource eventSource;

        public TimerEvent(TimerEventSource eventSource, string name, ScheduleInfo scheduleInfo) {
            Name = name;
            this.eventSource = eventSource;
            ScheduleInfo = scheduleInfo;
        }

        public string Name { get; }

        public ScheduleInfo ScheduleInfo { get; }

        IEventSource IEvent.Source => eventSource;

        public async Task<IEventContext> CreateContextAsync(ExecutionContext context)
        {
            var eventContext = new EventContext<TimerEvent>(this, context);
            await ((IEventSource)eventSource).AttachAsync(eventContext);
            return eventContext;
        }

        public void Dispose()
        {
        }
    }
}
