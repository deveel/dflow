using Deveel.Workflows.Events;
using System;

namespace Deveel.Workflows.Signals
{
    public sealed class Signal : IEventArgument
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

        EventType IEventArgument.EventType => EventType.Signal;
    }
}
