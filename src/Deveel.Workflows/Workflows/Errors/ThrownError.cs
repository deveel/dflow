using System;

namespace Deveel.Workflows.Errors
{
    public sealed class ThrownError : IError
    {
        public ThrownError(string processId, string instanceId, string errorName)
        {
            ProcessId = processId;
            InstanceId = instanceId;
            Name = errorName;
        }

        public string ProcessId { get; }

        public string InstanceId { get; }

        public string Name { get; }
    }
}
