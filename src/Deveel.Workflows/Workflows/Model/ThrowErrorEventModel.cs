using Deveel.Workflows.Errors;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Deveel.Workflows.Model
{
    public sealed class ThrowErrorEventModel : ThrowEventModel
    {
        public string ErrorName { get; set; }

        public string ErrorCode { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var errorSignaler = context.Context.GetRequiredService<IErrorSignaler>();

            return new ThrowEvent(Id, new ErrorEventRise(errorSignaler),
                new ThrownError(context.ProcessId, context.InstanceKey, ErrorName, ErrorCode));
        }
    }
}
