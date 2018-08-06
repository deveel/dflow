using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public abstract class Activity : FlowNode
    {
        private Action cancelHandle;
        private Action interruptHandle;

        protected Activity(string id) : base(id)
        {
            BoundaryEvents = new BoundaryEventCollection(this);
        }

        public BoundaryEventCollection BoundaryEvents { get; }

        internal override NodeContext CreateScope(NodeContext parent)
        {
            var scope = new ActivityContext(parent, this);
            cancelHandle = () => scope.Cancel();
            interruptHandle = () => scope.Interrupt();

            foreach (var boundaryEvent in BoundaryEvents)
            {
                scope.AddBoundaryEvent(boundaryEvent);
            }

            return scope;
        }

        internal void Interupt()
        {
            interruptHandle?.Invoke();
        }

        internal void CancelExecution()
        {
            cancelHandle?.Invoke();
        }
    }
}
