using System;

namespace Deveel.Workflows.Model
{
    public abstract class FlowNodeModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        internal abstract FlowNode BuildNode(ModelBuildContext context);
    }
}
