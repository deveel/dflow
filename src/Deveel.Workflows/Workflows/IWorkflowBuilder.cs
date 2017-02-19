using System;

namespace Deveel.Workflows {
	public interface IWorkflowBuilder {
		IWorkflowBuilder Activity(Action<IActivityBuilder> activity);

		IWorkflow Build(IBuildContext context);
	}
}