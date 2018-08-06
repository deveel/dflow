using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Deveel.Workflows.Model
{
    public sealed class ThrowErrorEventModel : EventModel
    {
        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var errorSignaler = context.Context.GetRequiredService<IErrorSignaler>();

            return new RiseEvent(Id, new ErrorEventRise(errorSignaler), new ThrownError(context.ProcessId, context.InstanceKey, Name));
        }
    }
}
