using System.Threading.Tasks;

namespace Deveel.Workflows.Scripts
{
    public interface IScriptingExecutor
    {
        string Format { get; }

        Task<ScriptResult> ExecuteAsync(ScriptGlobals globals);
    }
}
