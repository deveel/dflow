using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Messaging
{
    public sealed class Message
    {
        public Message()
        {
            Metadata = new Dictionary<string, object>();
        }

        public string Name { get; }

        public string ProcessId { get; }

        public string InstanceId { get; }

        public IDictionary<string, object> Metadata { get; set; }
    }
}
