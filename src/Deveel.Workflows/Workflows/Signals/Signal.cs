using System;

namespace Deveel.Workflows.Signals
{
    public sealed class Signal
    {
        public Signal(string name, string sender)
        {
            Name = name;
            Sender = sender;
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public string Name { get; }

        public DateTimeOffset TimeStamp { get; }

        public string Sender { get; }
    }
}
