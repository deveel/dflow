using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Errors;
using Deveel.Workflows.Events;
using Deveel.Workflows.Scripts;
using Deveel.Workflows.States;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ExecutionContext : ContextBase, IVariableContext
    {
        private readonly CancellationTokenSource tokenSource;

        private List<BoundaryEvent> events;

        public ExecutionContext(IContext parent, FlowNode node)
            : base(parent)
        {
            Node = node;

            tokenSource = new CancellationTokenSource();

            Process = FindProcess();

            Variables = new InMemoryVariableRegistry();
        }

        public ProcessContext Process { get; }

        public IContext Parent => ParentContext;

        public FlowNode Node { get; }

        public Exception Error { get; private set; }

        public ExecutionStatus Status { get; private set; }

        public DateTimeOffset? StartedAt { get; private set; }

        public DateTimeOffset? FinishedAt { get; private set; }

        public IVariableRegistry Variables { get; }

        public bool IsExecuting => Status == ExecutionStatus.Executing;

        public override CancellationToken CancellationToken => tokenSource.Token;

        internal void AddEvent(BoundaryEvent boundaryEvent)
        {
            if (events == null)
                events = new List<BoundaryEvent>();

            events.Add(boundaryEvent);
        }

        private ProcessContext FindProcess()
        {
            IContext context = ParentContext;
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

        internal async Task<bool> FailAsync(Exception error)
        {
            ChangeStatus(ExecutionStatus.Failed, error);

            if (error is IError) {
                var signal = this.GetService<IErrorSignaler>();
                if (signal != null)
                {
                    await signal.ThrowErrorAsync(new ThrownError(Process.Id, Process.InstanceId, ((IError)error).Name), CancellationToken);
                }

                return true;
            }

            return false;
        }

        internal void Complete()
            => ChangeStatus(ExecutionStatus.Completed);

        internal async Task StartAsync()
        {
            ChangeStatus(ExecutionStatus.Executing);

            if (events != null)
            {
                foreach (var e in events)
                    await e.BeginAsync();
            }
        }

        internal void Cancel()
        {
            tokenSource.Cancel();

            ChangeStatus(ExecutionStatus.Canceled);
        }

        internal void Interrupt()
        {
            // TODO: anything else to do here?
            ChangeStatus(ExecutionStatus.Interrupted);
        }

        public ExecutionContext CreateNodeContext(FlowNode node)
        {
            return new ExecutionContext(this, node);
        }

        public ScriptContext CreateScriptContext()
        {
            return new ScriptContext(this);
        }

        internal async Task<ExecutionState> GetStateAsync()
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && IsExecuting)
                Cancel();

            base.Dispose(disposing);
        }
    }
}
