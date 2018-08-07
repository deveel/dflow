using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class Process : ISequenceHandler
    {
        public Process(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
            Sequence = new ProcessSequence(this);
        }

        public ProcessSequence Sequence { get; }

        public ProcessInfo ProcessInfo { get; }

        ISequenceHandler ISequenceHandler.Parent {
            get => null;
            set => throw new NotSupportedException();
        }

        public async Task RunAsync(ProcessContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (var obj in Sequence)
            {
                if (context.IsRunning)
                {
                    var executionContext = context.CreateContext(obj);

                    await obj.ExecuteAsync(executionContext);
                }
            }
        }

        void ISequenceHandler.OnNodeAttached(FlowNode node)
        {
        }

        void ISequenceHandler.OnNodeDetached(FlowNode node)
        {
        }

        bool ISequenceHandler.NodeExists(FlowNode node) {
            return Sequence.Contains(node.Id);
        }
    }
}
