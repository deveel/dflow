using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Deveel.Workflows.Scripts
{
    public sealed class CSharpScriptEngine : IScriptEngine
    {
        public string Format => "csharp";

        public IScriptingExecutor CreateExecutor(string script, ScriptInfo scriptInfo)
        {
            var options = ScriptOptions.Default;
            if (scriptInfo.References != null)
                options = options.AddReferences(scriptInfo.References);

            options = options.WithImports("System.Dynamic");

            if (scriptInfo.Imports != null)
            {
                options = options.AddImports(scriptInfo.Imports);
            }

            return new Executor(CSharpScript.Create(script, options, typeof(ScriptGlobals)));
        }

        class Executor : IScriptingExecutor
        {
            private readonly Script script;

            public Executor(Script script)
            {
                this.script = script;
            }

            public string Format => "csharp";

            public async Task<ScriptResult> ExecuteAsync(ScriptGlobals globals)
            {
                var result = await script.RunAsync(globals);

                return new ScriptResult
                {
                    ReturnedValue = result.ReturnValue,
                    Variables = result.Variables.ToDictionary(x => x.Name, y => y.Value)
                };
            }
        }
    }
}
