﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Events;
using Deveel.Workflows.States;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ExecutionContext : IContext
    {
        public ExecutionContext(IContext parent, FlowNode node)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Provider = parent.CreateScope();
            Node = node;
            CancellationToken = CancellationToken.None;

            Process = FindProcess();
        }

        public IContext Parent { get; }

        private IServiceScope Provider { get; }

        public ProcessContext Process { get; }

        public FlowNode Node { get; }

        public IActor Actor { get; }

        public Exception Error { get; private set; }

        public ExecutionStatus Status { get; private set; }

        public DateTimeOffset? StartedAt { get; private set; }

        public DateTimeOffset? FinishedAt { get; private set; }

        public bool IsExecuting => Status == ExecutionStatus.Executing;

        public CancellationToken CancellationToken { get; set; }

        private ProcessContext FindProcess()
        {
            IContext context = Parent;
            while (context != null)
            {
                if (context is ProcessContext)
                    return (ProcessContext)context;

                context = context.Parent;
            }

            throw new InvalidOperationException();
        }

        private void ChangeStatus(ExecutionStatus status, Exception error = null)
        {
            Status = status;
            Error = error;

            if (status == ExecutionStatus.Executing)
                StartedAt = DateTimeOffset.UtcNow;
            else
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

        internal void Cancel()
            => ChangeStatus(ExecutionStatus.Canceled);

        public ExecutionContext CreateScope(FlowNode node)
        {
            return new ExecutionContext(this, node);
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

            var state = new ExecutionState(Process.Id, Process.InstanceId, Node.Id, Status, timeStamp);
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
