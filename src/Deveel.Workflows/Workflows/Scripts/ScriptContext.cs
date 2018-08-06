using System;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptContext : ContextBase
    {
        private IServiceScope scope;

        public ScriptContext(NodeContext context)
            : base(context)
        {
        }
    }
}
