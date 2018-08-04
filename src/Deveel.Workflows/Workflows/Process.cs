using System;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;

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

        public Task RunAsync(SystemContext context)
        {
            return RunAsync(context, Guid.NewGuid().ToString());
        }

        public Task RunAsync(SystemContext context, string instanceId)
        {
            return RunAsync(context, new SystemUser(), instanceId);
        }

        public Task RunAsync(SystemContext context, IActor actor)
        {
            return RunAsync(context, actor, Guid.NewGuid().ToString());
        }

        public Task RunAsync(SystemContext context, IActor actor, string instanceId)
        {
            return RunAsync(context, new NoneEvent(), actor, instanceId);
        }

        public Task RunAsync(SystemContext context, IEvent trigger)
        {
            return RunAsync(context, trigger, Guid.NewGuid().ToString());
        }

        public Task RunAsync(SystemContext context, IEvent trigger, string instanceId)
        {
            return RunAsync(context, trigger, new SystemUser(), instanceId);
        }

        public Task RunAsync(SystemContext context, IEvent trigger, IActor actor)
        {
            return RunAsync(context, trigger, actor, Guid.NewGuid().ToString());
        }

        public async Task RunAsync(SystemContext context, IEvent trigger, IActor actor, string instanceId)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var processContext = new ProcessContext(context, ProcessInfo, actor, trigger, instanceId);

            foreach(var obj in Sequence)
            {
                var executionContext = processContext.CreateContext(obj);

                await obj.ExecuteAsync(executionContext);
            }
        }
    }
}
