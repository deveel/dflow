using System;

namespace Deveel.Workflows.Scripts
{
    public interface IScriptEngine
    {
        string Format { get; }

        IScriptingExecutor CreateExecutor(string script, ScriptInfo scriptInfo);
    }
}
