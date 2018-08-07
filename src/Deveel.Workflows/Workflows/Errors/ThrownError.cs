using Deveel.Workflows.Events;
using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Errors
{
    public sealed class ThrownError : IError, IEventArgument
    {
        public ThrownError(string processId, string instanceId, string errorName, string errorCode)
        {
            ProcessId = processId;
            InstanceId = instanceId;
            Name = errorName;
            Code = errorCode;

            ErrorData = new Dictionary<string, object>();
        }

        public string ProcessId { get; }

        public string InstanceId { get; }

        public string Name { get; }

        public string Code { get; }

        EventType IEventArgument.EventType => EventType.Error;

        public IDictionary<string, object> ErrorData { get; set; }
    }
}
