using System;

namespace Deveel.Workflows
{
    public enum FlowNodeType
    {
        Task,
        Process,
        Transaction,
        Gateway,
        Event,
        BoundaryEvent
    }
}
