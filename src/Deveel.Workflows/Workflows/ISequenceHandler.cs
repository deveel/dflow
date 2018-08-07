using System;

namespace Deveel.Workflows
{
    interface ISequenceHandler {
        ISequenceHandler Parent { get; set; }


        bool NodeExists(FlowNode node);

        void OnNodeAttached(FlowNode node);

        void OnNodeDetached(FlowNode node);
    }
}
