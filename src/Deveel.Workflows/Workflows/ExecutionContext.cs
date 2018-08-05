using System;
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
    public sealed class ExecutionContext : IContext, IVariableContext
    {
        private readonly CancellationTokenSource tokenSource;
        private IServiceScope scope;

        public ExecutionContext(IContext parent, FlowNode node)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            scope = parent.CreateScope();
            Node = node;

            tokenSource = new CancellationTokenSource();
            CancellationToken = tokenSource.Token;

            Process = FindProcess();

            Variables = new InMemoryVariableRegistry();
        }

        public IContext Parent { get; }

        public ProcessContext Process { get; }

        public FlowNode Node { get; }

        public Exception Error { get; private set; }

        public ExecutionStatus Status { get; private set; }

        public DateTimeOffset? StartedAt { get; private set; }

        public DateTimeOffset? FinishedAt { get; private set; }

        public IVariableRegistry Variables { get; }

        public bool IsExecuting => Status == ExecutionStatus.Executing;

        public CancellationToken CancellationToken { get; }

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

        internal async Task<bool> FailAsync(Exception error)
        {
            ChangeStatus(ExecutionStatus.Failed, error);

            if (error is IError) {
                var handler = this.GetService<IErrorHandler>();
                if (handler != null)
                    await handler.ThrowErrorAsync(Process.Id, Process.InstanceId, (IError)error, CancellationToken);

                return true;
            }

            return false;
        }

        internal void Complete()
            => ChangeStatus(ExecutionStatus.Completed);

        internal void Start()
            => ChangeStatus(ExecutionStatus.Executing);

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

        public ExecutionContext CreateScope(FlowNode node)
        {
            return new ExecutionContext(this, node);
        }

        public ScriptContext CreateScript()
        {
            return new ScriptContext(this);
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
            return scope == null ? null : scope.ServiceProvider.GetService(serviceType);
        }

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
