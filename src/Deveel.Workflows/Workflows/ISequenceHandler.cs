using System;

namespace Deveel.Workflows
{
    interface ISequenceHandler
    {
        void OnNodeAttached(FlowNode node);

        void OnNodeDetached(FlowNode node);
    }
}
