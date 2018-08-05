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
        public ScriptTask(string id, string code, IScriptEngine engine, ScriptInfo scriptInfo) : base(id)
        {
            Engine = engine;
            Code = code;
            ScriptInfo = scriptInfo;
        }

        public IScriptEngine Engine { get; }

        public  string Code { get; }

        public ScriptInfo ScriptInfo { get; }

        protected override Task<object> CreateStateAsync(ExecutionContext context)
        {
            var scriptInfo = ScriptInfo.Generate(context, ScriptInfo);

            var executor = Engine.CreateExecutor(Code, scriptInfo);

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
