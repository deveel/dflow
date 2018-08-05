using Deveel.Workflows.Events;
using System;

namespace Deveel.Workflows.Signals
{
    public sealed class SignalEvent : Event
    {
        public SignalEvent(SignalEventSource source, string name) 
            : base(source, name)
        {
        }
    }
}
