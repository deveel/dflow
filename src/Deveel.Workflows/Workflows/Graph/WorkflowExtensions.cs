using System;
using System.Linq;

namespace Deveel.Workflows.Graph {
	public static class WorkflowExtensions  {
		public static ExecutionGraph Graph(this IWorkflowBuilder builder) {
			var grapBuilder = builder as IExecutionNodeBuilder;
			if (grapBuilder == null)
				throw new NotSupportedException();

			var rootNode = grapBuilder.BuildNode();
			var nodes = BuilderNode.BuildChain(rootNode.Nodes.Cast<ExecutionNode>());
			return new ExecutionGraph(nodes);
		}

		public static ExecutionGraph Graph(this IWorkflow workflow) {
			return new ExecutionGraph(ComponentNode.BuildNodes(workflow.Activities));
		}
	}
}