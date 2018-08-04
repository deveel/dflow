using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.States;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public abstract class FlowNode
    {
        protected FlowNode(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public abstract FlowNodeType NodeType { get; }

        protected virtual Task<object> CreateStateAsync(ExecutionContext context)
        {
            return Task.FromResult<object>(null);
        }

        internal Task<object> CallCreateStateAsync(ExecutionContext context)
            => CreateStateAsync(context);

        internal virtual ExecutionContext CreateScope(ExecutionContext parent)
        {
            return parent.CreateScope(this);
        }

        internal async Task ExecuteAsync(ExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scope = CreateScope(context);
            scope.Start();

            try
            {
                var state = await CreateStateAsync(context);
                await ExecuteNodeAsync(state, scope);

                scope.Complete();
            }
            catch (FlowException ex)
            {
                if (!await scope.FailAsync(ex))
                    throw;
            }
            catch (OperationCanceledException)
            {
                // ignore this
            }
            catch (Exception ex)
            {
                if (!await scope.FailAsync(ex))
                    throw new FlowException("The execution of the node failed", ex);
            }
            finally
            {
                await RegisterState(scope);
            }

        }

        private async Task RegisterState(ExecutionContext scope)
        {
            try
            {
                var registry = scope.GetService<IExecutionRegistry>();
                if (registry != null)
                await registry.RegisterAsync(await scope.GetStateAsync());
            }
            catch (Exception)
            {
                // TODO: log this error
            }

        }

        protected abstract Task ExecuteNodeAsync(object state, ExecutionContext context);

        internal Task CallExecuteNodeAsync(object state, ExecutionContext context)
        {
            return ExecuteNodeAsync(state, context);
        }
    }
}
