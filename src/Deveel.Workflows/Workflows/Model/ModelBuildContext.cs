using System;

namespace Deveel.Workflows.Model
{
    public sealed class ModelBuildContext
    {
        internal ModelBuildContext(IContext context, ProcessInfo processInfo)
        {
            Context = context;
            ProcessInfo = processInfo;
        }

        public ProcessInfo ProcessInfo { get; }

        public string ProcessId => ProcessInfo.Id;

        public string InstanceKey => ProcessInfo.InstanceKey;

        public IContext Context { get; }
    }
}
