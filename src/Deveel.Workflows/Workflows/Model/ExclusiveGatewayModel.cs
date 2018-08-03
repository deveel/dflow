using System;
using System.Collections.Generic;
using System.Linq;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public sealed class ExclusiveGatewayModel : GatewayModel
    {
        internal override Gateway CreateGateway(ModelBuildContext context, IEnumerable<OutGatewayFlow> flows)
        {
            var gateway = new ExclusiveGateway(Id);

            foreach (var flow in flows)
            {
                gateway.Flows.Add((ConditionalGatewayFlow) flow);
            }

            return gateway;
        }

        internal override IEnumerable<OutGatewayFlow> BuildFlows(ModelBuildContext context, IEnumerable<GatewayFlowModel> flowModels)
        {
            // TODO: validate the model

            return flowModels.Select(x =>
            {
                var obj = x.Activity.BuildNode(context);
                var condition = !String.IsNullOrWhiteSpace(x.Condition) ? FlowExpression.Parse(x.Condition) : null;

                return new ConditionalGatewayFlow(obj, condition);
            });
        }
    }
}
