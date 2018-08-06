using System;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;
using Deveel.Workflows.States;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public abstract class TaskTestsBase
    {
        protected TaskTestsBase() : this("testProcess")
        {
        }

        protected TaskTestsBase(string processId)
        {
            SystemContext = CreateSystemContext();
            Process = CreateProcess(processId);
            Context = CreateContext(Process);
        }

        protected Process Process { get; }

        protected ProcessContext Context { get; }

        protected SystemContext SystemContext { get; }

        private Process CreateProcess(string processId)
        {
            var process = new Process(new ProcessInfo(processId));
            OnTaskAdd(process.Sequence);

            return process;
        }

        private SystemContext CreateSystemContext()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IExecutionRegistry>(new InMemoryExecutionRegistry());

            AddServices(services);

            var provider = services.BuildServiceProvider();
            return new SystemContext(provider);
        }

        private ProcessContext CreateContext(Process process)
        {
            return SystemContext.CreateContext(process, new SystemUser(), new NoneEventSource("none"), Guid.NewGuid().ToString());
        }

        protected virtual void AddServices(IServiceCollection services)
        {

        }

        protected abstract void OnTaskAdd(ProcessSequence sequence);
    }
}
