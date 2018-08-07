using System;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Model
{
    public sealed class EscalationEventSourceModel : EventSourceModel
    {
        public  string Code { get; set; }

        internal override EventSource BuildSource(ModelBuildContext context) {
            throw new NotImplementedException();
        }
    }
}
