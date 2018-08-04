using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventSource : IEventSource
    {
        private IErrorSignal errorSignal;
        private Dictionary<string, ErrorEvent> events;
        private Dictionary<string, Task> waiters;
        private CancellationTokenSource tokenSource;

        public ErrorEventSource(IErrorSignal errorSignal)
        {
            this.errorSignal = errorSignal;
            events = new Dictionary<string, ErrorEvent>();
            waiters = new Dictionary<string, Task>();
            tokenSource = new CancellationTokenSource();
        }

        public EventType EventType => EventType.Error;

        void IEventSource.Attach(IEvent @event)
        {
            Attach((ErrorEvent)@event);
        }

        public void Attach(ErrorEvent @event)
        {
            if (!events.ContainsKey(@event.Name))
            {
                waiters[@event.Name] = Task.Run(() => WaitForErrorAsync(@event.Name), tokenSource.Token);
                events.Add(@event.Name, @event);
            }
        }

        internal void Detach(ErrorEvent @event)
        {
            if (events.ContainsKey(@event.Name))
            {
                var task = waiters[@event.Name];
                task.Dispose();

                waiters.Remove(@event.Name);
                events.Remove(@event.Name);
            }
        }

        private async Task WaitForErrorAsync(string errorName)
        {
            var error = await errorSignal.WaitForErrorAsync(errorName, tokenSource.Token);

            if (error != null && events.TryGetValue(error.Name, out ErrorEvent errorEvent))
            {
                errorEvent.HandleError(error);
            }
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            
            foreach(var e in events.Values)
            {
                e.Dispose();
            }

            events.Clear();
            errorSignal = null;
        }
    }
}
