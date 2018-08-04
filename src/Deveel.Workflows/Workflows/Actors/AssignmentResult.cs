using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Actors
{
    public class AssignmentResult
    {
        public AssignmentResult(bool completed, DateTimeOffset? completedAt = null)
        {
            if (completed && completedAt == null)
                completedAt = DateTimeOffset.UtcNow;

            Completed = completed;
            CompletedAt = completedAt;
            Metadata = new Dictionary<string, object>();
        }

        public DateTimeOffset? CompletedAt { get; }

        public bool Completed { get; }

        public IDictionary<string, object> Metadata { get; set; }
    }
}