using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deveel.Workflows.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class JoinGateway : Gateway
    {
        public JoinGateway(string id) 
            : this(id, (IMergeStrategy) null)
        {
        }

        public JoinGateway(string id, IMergeStrategy mergeStrategy) 
            : base(id)
        {
            MergeStrategy = mergeStrategy;
            Flows = new GatewayFlowCollection<InGatewayFlow>(this);
        }

        public JoinGateway(string id, Action<IExecutionContext> merge)
            : this(id, new DelegatedMergeStrategy(merge))
        {
        }

        public ICollection<InGatewayFlow> Flows { get; }

        public IMergeStrategy MergeStrategy { get; }

        protected override IEnumerator<IGatewayFlow> GetEnumerator()
        {
            return Flows.GetEnumerator();
        }

        internal override void ValidateAdd(IEnumerable<IGatewayFlow> flows, IGatewayFlow flow)
        {
            if (flows.Any(x => x.NodeRef == flow.NodeRef))
                throw new ArgumentException();
        }

        internal override async Task ExecuteNodeAsync(IExecutionContext context)
        {
            var registry = context.GetRequiredService<IExecutionRegistry>();

            await WaitForStatesAsync(registry);

            if (MergeStrategy != null)
                await MergeStrategy.MergeAsync(context);
        }

        private async Task WaitForStatesAsync(IExecutionRegistry listener)
        {
            var inputRefs = Flows.Select(x => x.ObjectRef).ToList();

            foreach (var id in inputRefs)
            {
                await listener.FindStateAsync(id);
            }
        }
    }
}
