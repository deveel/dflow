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

        private IScriptingExecutor Executor { get; set; }

        public override Task ExecuteAsync(IExecutionContext context)
        {
            Executor = Engine.CreateExecutor(Code, new ScriptInfo
            {
                Imports = Imports,
                References = References
            });

            return base.ExecuteAsync(context);
        }

        internal override async Task ExecuteNodeAsync(IExecutionContext context)
        {
            var registry = context.GetRequiredService<IVariableRegistry>();
            var variables = await registry.GetVariablesAsync();

            var globals = new ScriptGlobals(context, variables.ToDictionary(x => x.Name, x => x.Value));

            var result = Executor.ExecuteAsync(globals);

            if (globals.VariablesSet)
            {
                //TODO:
            }

            // TODO: set the result in scope
        }
    }
}
