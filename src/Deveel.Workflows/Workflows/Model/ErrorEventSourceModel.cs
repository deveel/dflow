using System;
using Deveel.Workflows.Events;
using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class ErrorEventSourceModel : EventSourceModel
    {
        public string ErrorName { get; set; }

        public string ErrorCode { get; set; }

        internal override EventSource BuildSource(ModelBuildContext context)
        {
            var errorSignal = context.Context.GetRequiredService<IErrorHandler>();
            return new ErrorEventSource(errorSignal, Id, ErrorName, ErrorCode);
        }
    }
}
