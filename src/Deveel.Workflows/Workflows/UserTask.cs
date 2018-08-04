using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class UserTask : TaskBase
    {
        public UserTask(string id, string assignee, DateTimeOffset? dueDate = null)
            : base(id) {
            Assignee = assignee;
            DueDate = dueDate;

            Metadata = new Dictionary<string, object>();
        }

        public string Assignee { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public IDictionary<string, object> Metadata { get; set; }

        protected override async Task<object> CreateStateAsync(ExecutionContext context)
        {
            var query = context.GetRequiredService<IUserQuery>();
            return await query.FindUserAsync(Assignee);
        }

        protected override async Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var registry = context.GetRequiredService<IAssignmentRegistry>();
            var user = (User) state;

            // TODO: get the process id
            var assignment = new UserAssignment(context.ProcessInfo.Id, Id, user, DueDate)
            {
                Metadata = new Dictionary<string, object>(Metadata)
            };

            // this blocks the process until the assignment is done
            var result = await registry.AssignAsync(assignment);

            if (!result.Completed)
                throw new InvalidOperationException();

            if (DueDate != null && result.CompletedAt > DueDate)
                throw new InvalidOperationException();

            // TODO: load into the metadata coming from the completed assignment
        }
    }
}
