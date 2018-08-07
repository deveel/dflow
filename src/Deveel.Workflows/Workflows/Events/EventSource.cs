using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public abstract class EventSource : IDisposable
    {
        protected EventSource(string id) {
            Id = id;
        }

        ~EventSource()
        {
            Dispose(false);
        }

        public string Id { get; }

        public abstract EventType EventType { get; }

        protected virtual EventContext OnNewEventContext(NodeContext context)
        {
            return new EventContext(context, this);
        }

        internal EventContext NewEventContext(NodeContext context)
        {
            return OnNewEventContext(context);
        }

        protected virtual Task AttachContextAsync(EventContext context)
        {
            return Task.CompletedTask;
        }

        protected virtual Task DetachContextAsync(EventContext context)
        {
            return Task.CompletedTask;
        }

        internal Task AttachAsync(EventContext context)
        {
            return AttachContextAsync(context);
        }

        internal Task DetachAsync(EventContext context)
        {
            return DetachContextAsync(context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }
}
