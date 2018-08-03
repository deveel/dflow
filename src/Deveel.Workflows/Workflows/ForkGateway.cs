using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Infrastructure;
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

        public override async Task ExecuteAsync(IExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var registry = context.GetRequiredService<IExecutionRegistry>();

            var scope = context.CreateScope();

            foreach (var obj in Flows)
            {
                await obj.Node.ExecuteAsync(scope);
            }

            await registry.RegisterAsync(Id, scope);
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
