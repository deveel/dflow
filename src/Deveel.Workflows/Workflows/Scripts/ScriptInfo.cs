using System.Collections.Generic;
using System.Reflection;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptInfo
    {
        public IEnumerable<string> Imports { get; set; }

        public IEnumerable<Assembly> References { get; set; }        
    }
}
