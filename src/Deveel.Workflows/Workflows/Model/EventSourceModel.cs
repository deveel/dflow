using System;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Model
{
    public abstract class EventSourceModel
    {
        public string Id { get; set; }

        internal abstract EventSource BuildSource(ModelBuildContext context);
    }
}
