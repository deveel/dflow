using System;
using Deveel.Workflows.Events;
using Deveel.Workflows.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class SignalEventSourceModel : EventSourceModel
    {
        public string SignalName { get; set; }

        internal override EventSource BuildSource(ModelBuildContext context)
        {
            var registry = context.Context.GetRequiredService<ISignalRegistry>();

            return new SignalEventSource(registry, SignalName);
        }
    }
}
