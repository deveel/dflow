using System;

namespace Deveel.Workflows.Model
{
    public sealed class ServiceTaskModel : TaskModel
    {
        public string ExecutorServiceType { get; set; }
    
        internal override Activity BuildActivity(ModelBuildContext context)
        {
            var serviceType = Type.GetType(ExecutorServiceType, false, true);

            if (serviceType == null)
                throw new InvalidOperationException($"Could not resolve type {ExecutorServiceType} within the scope of the system");

            if (!typeof(ITaskExecutor).IsAssignableFrom(serviceType))
                throw new InvalidOperationException($"The type '{serviceType}' is not an executor service.");

            var executor = context.Context.GetService(serviceType) as ITaskExecutor;

            return new ServiceTask(Id, executor);
        }
    }
}
