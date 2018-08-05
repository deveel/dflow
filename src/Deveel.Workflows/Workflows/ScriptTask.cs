using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Deveel.Workflows.Scripts;
using Deveel.Workflows.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class ScriptTask : TaskBase
    {
        public ScriptTask(string id, string code, IScriptEngine engine) : base(id)
        {
            Engine = engine;
            Code = code;
        }

        public IScriptEngine Engine { get; }

        public  string Code { get; }

        public IEnumerable<string> Imports { get; set; }

        public IEnumerable<Assembly> References { get; set; }

        protected override Task<object> CreateStateAsync(ExecutionContext context)
        {
            var executor = Engine.CreateExecutor(Code, new ScriptInfo
            {
                Imports = Imports,
                References = References
            });

            return Task.FromResult<object>(executor);
        }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var registry = context.GetRequiredService<IVariableRegistry>();
            var variables = await registry.GetVariablesAsync(context.CancellationToken);
            var executor = (IScriptingExecutor) state;

            var globals = context.CreateScript();

            var result = executor.ExecuteAsync(globals);


            // TODO: set the result in scope
        }
    }
}
