using System;
using System.Threading;

namespace Deveel.Workflows.Events
{
    public sealed class TimerEvent : IEvent, IDisposable
    {
        private Action<TimerEvent, ExecutionContext> callbacks;
        private ExecutionContext attachedContext;
        private TimerEventSource source;

        public TimerEvent(string name, TimerInfo info)
        {
            Name = name;
            TimerInfo = info;
            source = new TimerEventSource(info);
        }

        public string Name { get; }

        public TimerInfo TimerInfo { get; }

        IEventSource IEvent.Source => source;

        private void Elapsed()
        {
            callbacks?.Invoke(this, attachedContext);
        }

        void IEvent.AttachToContext(ExecutionContext context)
        {
            attachedContext = context;
            source.Attach(this);
        }

        public void Attach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks == null)
                callbacks = callback;
            else
            {
                callbacks = (Action<TimerEvent, ExecutionContext>) Delegate.Combine(callbacks, callback);
            }
        }

        public void Detach(Action<IEvent, ExecutionContext> callback)
        {
            if (callbacks != null)
                callbacks = (Action<TimerEvent, ExecutionContext>) Delegate.Remove(callbacks, callback);
        }

        public void Dispose()
        {
            source?.Dispose();
            source = null;
            callbacks = null;
        }

        #region TimerEventSource

        class TimerEventSource : IEventSource
        {
            private TimerEvent timerEvent;
            private Timer timer;
            private TimerInfo info;

            public TimerEventSource(TimerInfo info)
            {
                this.info = info;

            }

            public EventType EventType => EventType.Timer;

            private Timer CreateTimer()
            {
                TimeSpan dueTime;
                if (info.IsAbsoluteDate)
                {
                    dueTime = info.Date.ToUniversalTime().Subtract(DateTimeOffset.UtcNow);
                }
                else
                {
                    dueTime = info.Duration;
                }

                return new Timer(Elapsed, null, dueTime, TimeSpan.Zero);
            }

            private void Elapsed(object state)
            {
                timerEvent.Elapsed();
            }

            public void Attach(IEvent @event)
            {
                timerEvent = (TimerEvent) @event;
                timer = CreateTimer();
            }

            public void Dispose()
            {
                timer?.Dispose();
            }
        }

        #endregion
    }
}
