using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public abstract class Activity : FlowNode
    {
        private Action cancelHandle;

        protected Activity(string id) : base(id)
        {
            BoundaryEvents = new BoundaryEventCollection(this);
        }

        public BoundaryEventCollection BoundaryEvents { get; }

        internal override ExecutionContext CreateScope(ExecutionContext parent)
        {
            var scope = base.CreateScope(parent);
            cancelHandle = () => scope.Cancel();

            foreach (var boundaryEvent in BoundaryEvents)
            {
                boundaryEvent.Init(scope);
            }

            return scope;
        }

        internal void Interupt()
        {

        }

        internal void CancelExecution()
        {
            cancelHandle?.Invoke();
        }
    }
}
