using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class SubProcess : Activity
    {
        public SubProcess(string id) : base(id)
        {
            Sequence = new ProcessSequence();
        }

        public override FlowNodeType NodeType => FlowNodeType.Process;

        public ProcessSequence Sequence { get; }

        protected override async Task ExecuteNodeAsync(object state, NodeContext context)
        {
            foreach (var node in Sequence)
            {
                await node.ExecuteAsync(context);
            }
        }
    }
}
