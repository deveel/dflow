using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.States;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ExecutionContext : IContext
    {
        public ExecutionContext(IContext parent, IActor actor, ProcessInfo processInfo, FlowNode node) 
            : this(parent, new ManualEvent(), actor, processInfo, node)
        {
        }

        public ExecutionContext(IContext parent, IEvent trigger, IActor actor, ProcessInfo processInfo, FlowNode node)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            Provider = parent.CreateScope();
            ProcessInfo = processInfo;
            Node = node;
            CancellationToken = CancellationToken.None;
        }

        public ExecutionContext(ExecutionContext parent, FlowNode node)
            : this(parent, parent.Trigger, parent.Actor, parent.ProcessInfo, node)
        {
        }

        public IContext Parent { get; }

        private IServiceScope Provider { get; }

        public IActor Actor { get; }

        public IEvent Trigger { get; }

        public ProcessInfo ProcessInfo { get; }

        public FlowNode Node { get; }

        public Exception Error { get; private set; }

        public ExecutionStatus Status { get; private set; }

        public DateTimeOffset? StartedAt { get; private set; }

        public DateTimeOffset? FinishedAt { get; private set; }

        public bool IsExecuting => Status == ExecutionStatus.Executing;

        public CancellationToken CancellationToken { get; set; }

        private void ChangeStatus(ExecutionStatus status, Exception error = null)
        {
            Status = status;
            Error = error;

            if (status == ExecutionStatus.Executing)
                StartedAt = DateTimeOffset.UtcNow;
            else if (status == ExecutionStatus.Completed ||
                     status == ExecutionStatus.Failed)
                FinishedAt = DateTimeOffset.UtcNow;
        }

        internal void Fail(Exception error)
        {
            ChangeStatus(ExecutionStatus.Failed, error);
        }

        internal void Complete()
            => ChangeStatus(ExecutionStatus.Completed);

        internal void Start()
            => ChangeStatus(ExecutionStatus.Executing);

        public ExecutionContext CreateScope(FlowNode node)
        {
            return new ExecutionContext(this, Actor, ProcessInfo, node);
        }

        public async Task<ExecutionState> GetStateAsync()
        {
            DateTimeOffset timeStamp;

            if (Status == ExecutionStatus.Executing)
            {
                timeStamp = StartedAt ?? DateTimeOffset.UtcNow;
            } else {
                timeStamp = FinishedAt ?? DateTimeOffset.UtcNow;
            }

            var state = new ExecutionState(ProcessInfo.Id, Node.Id, Status, timeStamp);
            if (Status == ExecutionStatus.Failed)
                state.Error = Error;

            state.Variables = await this.GetVariablesAsync();

            return state;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return Provider == null ? null : Provider.ServiceProvider.GetService(serviceType);
        }
    }
}
