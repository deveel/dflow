using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Events
{
    public struct EventId : IEquatable<EventId>
    {
        public EventId(string processId, string instanceKey, string eventName)
        {
            ProcessId = processId;
            InstanceKey = instanceKey;
            EventName = eventName;
        }

        public string ProcessId { get; }

        public string InstanceKey { get; }

        public string EventName { get; }

        public bool Equals(EventId other)
        {
            return ProcessId == other.ProcessId &&
                InstanceKey == other.InstanceKey &&
                EventName == other.EventName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventId))
                return false;

            return base.Equals((EventId)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 900863140;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProcessId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InstanceKey);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventName);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{ProcessId}({InstanceKey}).{EventName}";
        }
    }
}
