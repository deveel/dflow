using System;

namespace Deveel.Workflows.Model
{
    public interface ITaskFactory
    {
        TaskBase CreateTask(TaskModel model);
    }
}
