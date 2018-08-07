using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Errors;
using Deveel.Workflows.States;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows {
    public class NodeContext : ContextBase {
        private readonly CancellationTokenSource tokenSource;

        public NodeContext(IContext parent, FlowNode node)
            : base(parent) {
            Node = node;

            tokenSource = new CancellationTokenSource();

            Process = FindProcess();
        }

        public ProcessContext Process { get; }

        public IContext Parent => ParentContext;

        public FlowNode Node { get; }

        public Exception Error { get; private set; }

        public ExecutionStatus Status { get; private set; }

        public DateTimeOffset? StartedAt { get; private set; }

        public DateTimeOffset? FinishedAt { get; private set; }

        public bool IsExecuting => Status == ExecutionStatus.Executing;

        public override CancellationToken CancellationToken => tokenSource.Token;

        private ProcessContext FindProcess() {
            IContext context = ParentContext;

            while (context != null) {
                if (context is ProcessContext)
                    return (ProcessContext) context;

                context = context.Parent;
            }

            throw new InvalidOperationException();
        }

        private void ChangeStatus(ExecutionStatus status, Exception error = null) {
            Status = status;
            Error = error;

            if (status == ExecutionStatus.Executing)
                StartedAt = DateTimeOffset.UtcNow;
            else
                FinishedAt = DateTimeOffset.UtcNow;
        }

        internal async Task<bool> FailAsync(Exception error) {
            ChangeStatus(ExecutionStatus.Failed, error);

            try {
                if (error is IError) {
                    var signal = this.GetService<IErrorSignaler>();

                    if (signal != null) {
                        await signal.ThrowErrorAsync(
                            new ThrownError(Process.Id, Process.InstanceKey, ((IError) error).Name, ((IError)error).Code), CancellationToken);
                    }

                    return true;
                }
            }
            finally {
                Node.OnExecutionFailed(this);
            }

            return false;
        }

        internal void Complete()
            => ChangeStatus(ExecutionStatus.Completed);

        internal virtual Task StartAsync() {
            ChangeStatus(ExecutionStatus.Executing);

            return Task.CompletedTask;
        }

        internal void Cancel() {
            tokenSource.Cancel();

            ChangeStatus(ExecutionStatus.Canceled);
        }

        internal void Interrupt() {
            // TODO: anything else to do here?
            ChangeStatus(ExecutionStatus.Interrupted);
        }

        public NodeContext CreateScope(FlowNode node) {
            return new NodeContext(this, node);
        }

        internal async Task<ExecutionState> GetStateAsync() {
            DateTimeOffset timeStamp;

            if (Status == ExecutionStatus.Executing) {
                timeStamp = StartedAt ?? DateTimeOffset.UtcNow;
            }
            else {
                timeStamp = FinishedAt ?? DateTimeOffset.UtcNow;
            }

            var state = new ExecutionState(Process.Id, Process.InstanceKey, Node.Id, Status, timeStamp);
            if (Status == ExecutionStatus.Failed)
                state.Error = Error;

            state.Variables = await this.GetVariablesAsync();

            return state;
        }

        protected override void Dispose(bool disposing) {
            if (disposing && IsExecuting)
                Cancel();

            base.Dispose(disposing);
        }
    }
}