using System;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Escalations
{
    public sealed class Escalation : IEventArgument
    {
        public Escalation(string processId, string instanceKey) : this(processId, instanceKey, null) {
        }

        public Escalation(string processId, string instanceKey, string code) {
            if (string.IsNullOrWhiteSpace(processId))
                throw new ArgumentException("message", nameof(processId));
            if (string.IsNullOrWhiteSpace(instanceKey))
                throw new ArgumentException("message", nameof(instanceKey));

            ProcessId = processId;
            InstanceKey = instanceKey;
            Code = code;
        }

        public  string Code { get; }

        public string ProcessId { get; }

        public string InstanceKey { get; }

        EventType IEventArgument.EventType => EventType.Escalation;
    }
}
