using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class UserTask : TaskBase
    {
        public UserTask(string id, User user, DateTimeOffset? dueDate)
            : base(id) {
            User = user;
            DueDate = dueDate;

            Metadata = new Dictionary<string, object>();
        }

        public User User { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public IDictionary<string, object> Metadata { get; }

        internal override async Task ExecuteNodeAsync(IExecutionContext context)
        {
            var registry = context.GetRequiredService<IAssignmentRegistry>();
            var assignment = new UserAssignment(User, DueDate)
            {
                Metadata = new Dictionary<string, object>(Metadata)
            };

            await registry.CreateAssignmentAsync(assignment);
        }
    }
}
