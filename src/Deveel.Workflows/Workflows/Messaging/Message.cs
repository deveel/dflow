using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Messaging
{
    public sealed class Message : IEventArgument
    {
        public Message(string processId, string instanceId, string name)
        {
            Name = name;
            ProcessId = processId;
            InstanceId = instanceId;
            Metadata = new Dictionary<string, object>();
        }

        EventType IEventArgument.EventType => EventType.Message;

        public string Name { get; }

        public string ProcessId { get; }

        public string InstanceId { get; }

        public IDictionary<string, object> Metadata { get; set; }
    }
}
