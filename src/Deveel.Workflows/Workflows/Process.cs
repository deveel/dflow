using System;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;

namespace Deveel.Workflows
{
    public sealed class Process
    {
        public Process(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
            Sequence = new ProcessSequence();
        }

        public Process(string id)
            : this(new ProcessInfo(id))
        {
        }

        public ProcessSequence Sequence { get; }

        public ProcessInfo ProcessInfo { get; }

        public Task RunAsync(IContext context)
        {
            return RunAsync(context, new SystemUser());
        }

        public Task RunAsync(IContext context, IActor actor)
        {
            return RunAsync(context, new ManualEvent(), actor);
        }

        public Task RunAsync(IContext context, IEvent trigger)
        {
            return RunAsync(context, trigger, new SystemUser());
        }

        public async Task RunAsync(IContext context, IEvent trigger, IActor actor)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach(var obj in Sequence)
            {
                var executionContext = new ExecutionContext(context, trigger, actor, ProcessInfo, obj);
                await obj.ExecuteAsync(executionContext);
            }
        }
    }
}
