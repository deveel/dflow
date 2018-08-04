using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public abstract class Event : FlowNode, IEvent
    {
        protected Event(string id, string name)
        : base(id)
        {
            Name = name;
        }

        public string Name { get; }

        public override FlowNodeType NodeType => FlowNodeType.Event;
    }
}
