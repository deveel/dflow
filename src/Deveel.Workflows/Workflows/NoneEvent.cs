using System;

namespace Deveel.Workflows
{
    public sealed class NoneEvent : Event
    {
        public NoneEvent(string id, string name) 
            : base(id, name)
        {
        }
    }
}
