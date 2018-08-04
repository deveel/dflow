using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deveel.Workflows.States;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ForkGateway : Gateway
    {
        public ForkGateway(string id) 
            : base(id)
        {
            Flows = new GatewayFlowCollection<SimpleGatewayFlow>(this);
        }

        public ICollection<SimpleGatewayFlow> Flows { get; }

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var tasks = Flows.Select(x => x.Node.ExecuteAsync(context));
            return Task.WhenAll(tasks);
        }

        protected override IEnumerator<IGatewayFlow> GetEnumerator()
        {
            return Flows.GetEnumerator();
        }

        internal override void ValidateAdd(IEnumerable<IGatewayFlow> flows, IGatewayFlow item)
        {
        }
    }
}
