using System;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Model
{
    public abstract class EventSourceModel
    {
        public string EventName { get; set; }

        internal abstract EventSource BuildSource(ModelBuildContext context);
    }
}
