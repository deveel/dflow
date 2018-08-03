using System;
using System.Threading.Tasks;
using Deveel.Workflows.Infrastructure;
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

        public virtual async Task ExecuteAsync(IExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var registry = context.GetRequiredService<IExecutionRegistry>();
            var scope = context.CreateScope();

            await ExecuteNodeAsync(scope);

            await registry.RegisterAsync(Id, scope);
        }

        internal virtual Task ExecuteNodeAsync(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
