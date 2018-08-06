using System;

namespace Deveel.Workflows
{
    public sealed class ProcessInfo
    {
        public ProcessInfo(string id, string instanceKey)
        {
            Id = id;
            InstanceKey = instanceKey;
        }

        public string Id { get; }

        public string InstanceKey { get; }

        public string Name { get; set; }        
    }
}
