using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class Process : Activity
    {
        public Process(string id)
        : base(id)
        {
            Sequence = new ProcessSequence(this);
        }

        public override FlowNodeType NodeType => FlowNodeType.Process;

        public ProcessSequence Sequence { get; }


        public override async Task ExecuteAsync(IExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var registry = context.GetRequiredService<IExecutionRegistry>();

            var scope = context.CreateScope();

            foreach(var obj in Sequence)
            {
                await obj.ExecuteAsync(scope);
            }

            await registry.RegisterAsync(Id, scope);
        }
    }
}
