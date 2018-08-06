using Deveel.Workflows.Scripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public class ActivityContext : ExecutionContext
    {
        private List<BoundaryEvent> events;

        public ActivityContext(IContext parent, Activity node) : base(parent, node)
        {
        }

        internal void AddEvent(BoundaryEvent boundaryEvent)
        {
            if (events == null)
                events = new List<BoundaryEvent>();

            events.Add(boundaryEvent);
        }

        internal async override Task StartAsync()
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
