using System;

namespace Deveel.Workflows
{
    public abstract class TaskBase : Activity
    {
        protected TaskBase(string id)
            : base(id) {
        }

        public override FlowNodeType NodeType => FlowNodeType.Task;
    }
}
