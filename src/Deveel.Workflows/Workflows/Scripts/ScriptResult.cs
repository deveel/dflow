using System.Collections.Generic;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptResult
    {
        public ScriptResult()
        {
            Variables = new Dictionary<string, object>();
        }

        public object ReturnedValue { get; set; }

        public IDictionary<string, object> Variables { get; set; }
    }
}
