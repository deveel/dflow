using System;
using Deveel.Workflows.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class UserTaskModel : TaskModel
    {
        public string Assignee { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        internal override TaskBase BuildTask(ModelBuildContext context)
        {
            return new UserTask(Id, Assignee, DueDate);
        }
    }
}
