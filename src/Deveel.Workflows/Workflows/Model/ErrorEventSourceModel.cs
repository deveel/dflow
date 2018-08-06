using System;
using Deveel.Workflows.Events;
using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class ErrorEventSourceModel : EventSourceModel
    {
        internal override EventSource BuildSource(ModelBuildContext context)
        {
            var errorSignal = context.Context.GetRequiredService<IErrorHandler>();
            return new ErrorEventSource(errorSignal, EventName);
        }
    }
}
