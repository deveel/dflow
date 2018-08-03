using System;
using Deveel.Workflows.Actors;
using Deveel.Workflows.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class UserTaskModel : TaskModel
    {
        public string Assignee { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        internal override TaskBase BuildTask(ModelBuildContext context)
        {
            var query = context.Context.GetRequiredService<IUserQuery>();
            var actor = query.FindUserAsync(Assignee).Result;

            return new UserTask(Id, actor, DueDate);
        }
    }
}
