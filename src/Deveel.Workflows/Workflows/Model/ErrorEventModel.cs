using System;
using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class ErrorEventModel : EventModel
    {
        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var errorSignal = context.Context.GetRequiredService<IErrorSignal>();
            var source = new ErrorEventSource(errorSignal);
            var errorEvent = new ErrorEvent(source, Name);

            return new Event(Id, errorEvent);
        }
    }
}
