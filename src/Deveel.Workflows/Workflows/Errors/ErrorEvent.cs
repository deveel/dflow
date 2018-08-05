using Deveel.Workflows.Events;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEvent : FlowEventHandler
    {
        public ErrorEvent(ErrorEventSource source, string name)
            : base(source, name)
        {
        }
    }
}
