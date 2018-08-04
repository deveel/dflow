using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Actors
{
    public sealed class UserAssignment
    {
        public UserAssignment(string processId, string taskId, User assignee, DateTimeOffset? dueDate)
        {
            ProcessId = processId;
            TaskId = taskId;
            Assignee = assignee;
            DueDate = dueDate;
            Metadata = new Dictionary<string, object>();
        }

        public string ProcessId { get; }

        public string TaskId { get; }

        public User Assignee { get; }

        public DateTimeOffset? DueDate { get; }

        public IDictionary<string, object> Metadata { get; set; }
    }
}
