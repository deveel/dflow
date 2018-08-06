using System;
using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class CatchErrorEventModel : EventModel
    {
        public string ErrorVariableName { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var errorSignal = context.Context.GetRequiredService<IErrorHandler>();
            var source = new ErrorEventSource(errorSignal);
            var errorEvent = new ErrorEventHandler(source, Name);

            return new CatchEvent(Id, errorEvent, ErrorVariableName);
        }
    }
}
