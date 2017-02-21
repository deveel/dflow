using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Graph {
	public sealed class ExecutionGraph : IExecutionNode {
		internal ExecutionGraph(IEnumerable<IExecutionNode> nodes) {
			Nodes = nodes;
		}

		string IExecutionNode.Name => null;

		IExecutionNode IExecutionNode.Previous => null;

		IExecutionNode IExecutionNode.Next => null;

		IDictionary<string, object> IExecutionNode.Metadata => new Dictionary<string, object>();

		public IEnumerable<IExecutionNode> Nodes { get; }

		bool IExecutionNode.HasDecision => false;
	}
}