using Deveel.Workflows.Scripts;
using Deveel.Workflows.Variables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public class ActivityContext : NodeContext, IVariableContext
    {
        private List<BoundaryEvent> events;

        public ActivityContext(IContext parent, Activity node) : base(parent, node)
        {
            Variables = new InMemoryVariableRegistry();
        }

        public IVariableRegistry Variables { get; }


        internal void AddBoundaryEvent(BoundaryEvent boundaryEvent)
        {
            if (events == null)
                events = new List<BoundaryEvent>();

            events.Add(boundaryEvent);
        }

        internal override async Task StartAsync()
        {
            await base.StartAsync();

            if (events != null)
            {
                foreach (var e in events)
                    await e.ExecuteAsync(this);
            }
        }
    }
}
