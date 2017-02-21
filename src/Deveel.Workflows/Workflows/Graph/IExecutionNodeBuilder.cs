using System;

namespace Deveel.Workflows.Graph {
	interface IExecutionNodeBuilder {
		ExecutionNode BuildNode();
	}
}