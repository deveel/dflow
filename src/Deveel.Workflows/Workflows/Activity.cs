using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Workflows.States;

using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows {
    public abstract class Activity : FlowNode {
        private Action cancelHandle;
        private Action interruptHandle;

        protected Activity(string id) : base(id) {
            BoundaryEvents = new BoundaryEventCollection(this);
        }

        public BoundaryEventCollection BoundaryEvents { get; }

        protected virtual ActivityContext CreateScope(NodeContext parent) {
            var scope = new ActivityContext(parent, this);
            cancelHandle = () => scope.Cancel();
            interruptHandle = () => scope.Interrupt();

            foreach (var boundaryEvent in BoundaryEvents) {
                scope.AddBoundaryEvent(boundaryEvent);
            }

            return scope;
        }

        protected override async Task ExecuteNodeAsync(object state, NodeContext context) {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scope = CreateScope(context);

            try {
                await scope.StartAsync();

                await ExecuteActivityAsync(scope, state);

                scope.Complete();
            }
            catch (FlowException ex) {
                if (!await scope.FailAsync(ex))
                    throw;
            }
            catch (OperationCanceledException) {
                // ignore this
            }
            catch (Exception ex) {
                if (!await scope.FailAsync(ex))
                    throw new FlowException("The execution of the node failed", ex);
            }
            finally {
                await RegisterState(scope);

                scope.Dispose();
            }

        }

        private async Task RegisterState(NodeContext scope) {
            try {
                var registry = scope.GetService<IExecutionRegistry>();
                if (registry != null)
                    await registry.RegisterAsync(await scope.GetStateAsync());
            }
            catch (Exception) {
                // TODO: log this error
            }

        }


        protected abstract Task ExecuteActivityAsync(ActivityContext context, object state);

        internal void Interupt() {
            interruptHandle?.Invoke();
        }

        internal void CancelExecution() {
            cancelHandle?.Invoke();
        }
    }
}