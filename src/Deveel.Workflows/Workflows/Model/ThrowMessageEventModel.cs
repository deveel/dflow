using Deveel.Workflows.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Deveel.Workflows.Model
{
    public sealed class ThrowMessageEventModel : ThrowEventModel
    {
        public string MessageName { get; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var message = new Message(context.ProcessId, context.InstanceKey, MessageName);
            var publisher = context.Context.GetRequiredService<IMessagePublisher>();

            return new ThrowEvent(Id, new MessageEventRise(publisher), message);
        }
    }
}
