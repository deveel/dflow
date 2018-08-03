using System;

namespace Deveel.Workflows
{
    public abstract class Activity : FlowNode
    {
        protected Activity(string id) : base(id)
        {
        }
    }
}
