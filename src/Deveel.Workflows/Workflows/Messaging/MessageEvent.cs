using Deveel.Workflows.Events;
using System.Collections.Generic;

namespace Deveel.Workflows.Messaging
{
    public sealed class MessageEvent : FlowEventHandler
    {
        public MessageEvent(MessageEventSource source, string name, IDictionary<string, object> parameters)
            : base(source, name)
        {
            // TODO: use a different receiver name?
            Subscription = new MessageSubscription(name)
            {
                Parameters = parameters
            };
        }

        public MessageSubscription Subscription { get; }
    }
}
