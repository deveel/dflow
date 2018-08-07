using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public sealed class Process : ISequenceHandler, IDisposable
    {
        public Process(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
            Sequence = new ProcessSequence(this);
        }

        public ProcessSequence Sequence { get; }

        public ProcessInfo ProcessInfo { get; }

        public void Dispose()
        {
            
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
    }
}
