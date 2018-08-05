using Deveel.Workflows.Events;
using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class ErrorEventHandler : FlowEventHandler
    {
        public ErrorEventHandler(ErrorEventSource source, string name)
            : base(source, name)
        {
        }
    }
}
