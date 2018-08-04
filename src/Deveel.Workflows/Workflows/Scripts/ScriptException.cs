using System;

namespace Deveel.Workflows.Scripts
{
    public class ScriptException : FlowException
    {
        public ScriptException()
        {
        }

        public ScriptException(string message) : base(message)
        {
        }

        public ScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
