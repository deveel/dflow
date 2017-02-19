using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows {
	class WorkflowBuilder : IWorkflowBuilder {
		private List<IActivityBuilder> builders;

		public WorkflowBuilder() {
			builders = new List<IActivityBuilder>();
		}

		public IWorkflowBuilder Activity(Action<IActivityBuilder> activity) {
			var builder = new ActivityBuilder();
			activity(builder);

			builders.Add(builder);

			return this;
		}

		public IWorkflow Build(IBuildContext context) {
			var activities = builders.Select(x => x.Build(context));

			var workflow = new Workflow();
			foreach (var activity in activities) {
				workflow.Add(activity);
			}

			return workflow;
		}
	}
}