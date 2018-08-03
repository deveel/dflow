using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Actors
{
    public sealed class UserAssignment
    {
        public UserAssignment(User assignee, DateTimeOffset? dueDate)
        {
            Assignee = assignee;
            DueDate = dueDate;
            Metadata = new Dictionary<string, object>();
        }

        public User Assignee { get; }

        public DateTimeOffset? DueDate { get; }

        public IDictionary<string, object> Metadata { get; set; }
    }
}
