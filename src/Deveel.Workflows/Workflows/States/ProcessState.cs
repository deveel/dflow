using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Workflows.States
{
    public sealed class ProcessState
    {
        public ProcessState(string processId, string instanceId, ProcessStatus status, DateTimeOffset timeStamp)
        {
            ProcessId = processId;
            InstanceId = instanceId;
            Status = status;
            TimeStamp = timeStamp;
        }

        public string ProcessId { get; }

        public string InstanceId { get; }

        public ProcessStatus Status { get; }

        public DateTimeOffset TimeStamp { get; }
    }
}
