using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class ExclusiveGateway : Gateway
    {
        public ExclusiveGateway(string id) 
            : base(id)
        {
            Flows = new GatewayFlowCollection<ConditionalGatewayFlow>(this);
        }

        public ICollection<ConditionalGatewayFlow> Flows { get; }

        protected override IEnumerator<IGatewayFlow> GetEnumerator()
        {
            return Flows.GetEnumerator();
        }

        internal override void ValidateAdd(IEnumerable<IGatewayFlow> flows, IGatewayFlow item)
        {
            var flow = (ConditionalGatewayFlow) item;
            if (flow.Condition == null && 
                flows.Cast<ConditionalGatewayFlow>().Any(x => x.Condition == null))
            {
                throw new ArgumentException();
            }
        }

        public override async Task ExecuteAsync(IExecutionContext context)
        {
            var conditionsFlows = Flows.Where(x => x.Condition != null);
            var elseFlow = Flows.SingleOrDefault(x => x.Condition == null);

            foreach(var flow in conditionsFlows)
            {
                if (await flow.Condition.IsTrueAsync(context))
                {
                    await flow.Node.ExecuteAsync(context);
                    return;
                }
            }

            if (elseFlow == null)
                throw new InvalidOperationException();

            await elseFlow.Node.ExecuteAsync(context);
        }
    }
}
