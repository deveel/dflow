using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Events
{
    public abstract class FlowEventSource : IDisposable
    {
        ~FlowEventSource()
        {
            Dispose(false);
        }

        public abstract EventType EventType { get; }

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
