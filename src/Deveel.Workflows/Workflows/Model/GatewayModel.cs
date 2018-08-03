using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Model
{
    public abstract class GatewayModel : FlowNodeModel
    {
        public GatewayModel()
        {
            Flows = new List<GatewayFlowModel>();
        }

        public ICollection<GatewayFlowModel> Flows { get; set; }

        internal abstract IEnumerable<OutGatewayFlow> BuildFlows(ModelBuildContext context, IEnumerable<GatewayFlowModel> flowModels);

        internal virtual bool ValidateFlows(out Exception error)
        {
            error = null;
            return true;
        }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            if (!ValidateFlows(out var error))
                throw error;

            var flows = BuildFlows(context, Flows.AsEnumerable());

            return CreateGateway(context, flows);
        }

        internal abstract Gateway CreateGateway(ModelBuildContext context, IEnumerable<OutGatewayFlow> flows);
    }
}
