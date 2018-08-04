using System;
using System.Collections.Generic;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows.States
{
    public sealed class ExecutionState
    {
        public ExecutionState(string processId, string processInstanceId, string nodeId, ExecutionStatus status, DateTimeOffset timeStamp)
        {
            ProcessId = processId;
            ProcessInstanceId = processInstanceId;
            NodeId = nodeId;
            TimeStamp = timeStamp.ToUniversalTime();
            Status = status;
        }

        public string ProcessId { get; }

        public string ProcessInstanceId { get; }

        public string NodeId { get; }

        public DateTimeOffset TimeStamp { get; }

        public ExecutionStatus Status { get; }

        public IEnumerable<Variable> Variables { get; set; }

        public Exception Error { get; set; }
    }
}
