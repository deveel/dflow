using System;
using System.Collections.Generic;
using System.Threading;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ProcessContext : IContext, IVariableContext
    {
        private IServiceScope scope;
        private CancellationTokenSource tokenSource;
        private List<ExecutionContext> currentContexts;

        internal ProcessContext(SystemContext parent, Process process, IActor actor, EventSource trigger, string instanceId)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Process = process;
            Actor = actor;
            Trigger = trigger;
            scope = parent.CreateScope();
            InstanceId = instanceId;

            tokenSource = new CancellationTokenSource();
            CancellationToken = tokenSource.Token;

            currentContexts = new List<ExecutionContext>();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return scope.ServiceProvider.GetService(serviceType);
        }

        public IContext Parent { get; }

        public CancellationToken CancellationToken { get; }

        public string InstanceId { get; }

        public Process Process { get; }

        public IVariableRegistry Variables => scope.ServiceProvider.GetRequiredService<IVariableRegistry>();

        public ProcessStatus Status { get; private set; }


        public bool IsRunning => Status == ProcessStatus.Running;

        public string Id => Process.ProcessInfo.Id;

        public IActor Actor { get; }

        public EventSource Trigger { get; }

        public ExecutionContext CreateContext(FlowNode node)
        {
            return new ExecutionContext(this, node);
        }

        internal void Attach(ExecutionContext context)
        {
            currentContexts.Add(context);
        }

        internal void Detach(ExecutionContext context)
        {
            currentContexts.Remove(context);
        }

        private void ChangeStatus(ProcessStatus status)
        {
            Status = status;
        }

        internal void Start()
            => ChangeStatus(ProcessStatus.Running);

        internal void Complete()
            => ChangeStatus(ProcessStatus.Completed);

        public void Cancel()
        {
            if (IsRunning)
            {
                foreach (var context in currentContexts)
                {
                    context.Cancel();
                }

                tokenSource.Cancel();

                ChangeStatus(ProcessStatus.Cancelled);
            }
        }

        public void Dispose()
        {
            Cancel();
            scope?.Dispose();
        }
    }
}
