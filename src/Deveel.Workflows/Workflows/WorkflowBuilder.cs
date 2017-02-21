using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Workflows.Graph;

namespace Deveel.Workflows {
	class WorkflowBuilder : IWorkflowBuilder, IExecutionNodeBuilder {
		private List<ActivityBuilder> builders;

		public WorkflowBuilder() {
			builders = new List<ActivityBuilder>();
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

		public ExecutionNode BuildNode() {
			return new BuilderNode(null, false, new KeyValuePair<string, object>[0]) {
				InnerNodes = builders.Select(x => x.BuildNode())
			};
		}
	}
}