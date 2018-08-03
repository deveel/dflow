using System;

namespace Deveel.Workflows.Model
{
    public sealed class ModelBuildContext
    {
        internal ModelBuildContext(string processId, IContext context)
        {
            ProcessId = processId;
            Context = context;
        }

        public string ProcessId { get; }

        public IContext Context { get; }
    }
}
