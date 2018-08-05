using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Messaging
{
    public sealed class MessageSubscription : IEquatable<MessageSubscription>
    {
        public MessageSubscription(string messageName)
        {
            if (string.IsNullOrWhiteSpace(messageName))
            {
                throw new ArgumentException("message", nameof(messageName));
            }

            MessageName = messageName;
            Parameters = new Dictionary<string, object>();
        }

        public string MessageName { get; }

        public IDictionary<string, object> Parameters { get; set; }

        public bool Equals(MessageSubscription other)
        {
            if (other == null)
                return false;

            if (!String.Equals(MessageName, other.MessageName))
                return false;

            if (other.Parameters == null && Parameters == null)
                return true;
            if (other.Parameters == null && Parameters != null)
                return false;
            if (other.Parameters != null && Parameters == null)
                return false;

            foreach(var pair in Parameters)
            {
                object value;
                if (!other.Parameters.TryGetValue(pair.Key, out value))
                    return false;
                if (Equals(pair.Value, value))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MessageSubscription))
                return false;

            return Equals((MessageSubscription)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -72963664;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDictionary<string, object>>.Default.GetHashCode(Parameters);
            return hashCode;
        }
    }
}
