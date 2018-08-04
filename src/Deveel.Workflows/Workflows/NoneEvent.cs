using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class NoneEvent : Event
    {
        public NoneEvent(string id, string name) 
            : base(id, name)
        {
        }

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}
