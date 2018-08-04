using System;
using System.Collections.Generic;
using System.Text;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public abstract class TaskTestsBase
    {
        protected TaskTestsBase(string processId)
        {
            Process = CreateProcess(processId);
            Context = CreateContext();
        }

        protected Process Process { get; }

        protected IExecutionContext Context { get; }

        private Process CreateProcess(string processId)
        {
            var process = new Process(processId);
            OnTaskAdd(process.Sequence);

            return process;
        }

        private IExecutionContext CreateContext()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IExecutionRegistry>(new InMemoryExecutionRegistry());

            AddServices(services);

            var provider = services.BuildServiceProvider();
            return new ExecutionContext(new SystemUser(), provider.CreateScope());
        }

        protected virtual void AddServices(IServiceCollection services)
        {

        }

        protected abstract void OnTaskAdd(ProcessSequence sequence);
    }
}
