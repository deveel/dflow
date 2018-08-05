using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Deveel.Workflows.Variables;
using System.Collections.Generic;

namespace Deveel.Workflows.Scripts
{
    public sealed class CSharpScriptEngine : IScriptEngine
    {
        string IScriptEngine.Format => "csharp";

        public IScriptingExecutor CreateExecutor(string script, ScriptInfo scriptInfo)
        {
            try
            {
                var options = ScriptOptions.Default;
            if (scriptInfo.References != null)
                options = options.AddReferences(scriptInfo.References);

            options = options.AddReferences(typeof(IContext).Assembly)
                .AddReferences(typeof(DynamicObject).Assembly)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly)
                .AddReferences(typeof(ExpandoObject).Assembly);

            options = options.AddImports("Deveel.Workflows")
                .AddImports("Deveel.Workflows.Variables")
                .AddImports("Deveel.Workflows.Scripts");

            if (scriptInfo.Imports != null)
            {
                options = options.AddImports(scriptInfo.Imports);
            }

                return new Executor(CSharpScript.Create(script, options, typeof(CSharpGlobals)));
            }
            catch (Exception ex)
            {
                throw new ScriptException("Cannot create the csharp executor", ex);
            }
        }

        class Executor : IScriptingExecutor
        {
            private readonly Script script;

            public Executor(Script script)
            {
                this.script = script;
            }

            public string Format => "csharp";

            public async Task<ScriptResult> ExecuteAsync(ScriptContext context)
            {
                try
                {
                    var globals = new CSharpGlobals(context);

                    var result = await script.RunAsync(globals);

                    return new ScriptResult
                    {
                        ReturnedValue = result.ReturnValue,
                        //Variables = result.Variables.ToDictionary(x => x.Name, y => y.Value)
                    };
                } catch(ScriptException) {
                    throw;
                } catch(Exception ex)
                {
                    throw new ScriptException("Error while executing script", ex);
                }
            }
        }
    }
}
