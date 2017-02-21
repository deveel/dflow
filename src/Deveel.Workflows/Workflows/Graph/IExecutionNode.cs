using System.Collections.Generic;

namespace Deveel.Workflows.Graph {
	public interface IExecutionNode {
		string Name { get; }

		bool HasDecision { get; }

		bool IsBranch { get; }

		bool IsParallel { get; }

		IExecutionNode Previous { get; }

		IExecutionNode Next { get; }

		IDictionary<string, object> Metadata { get; }

		IEnumerable<IExecutionNode> Nodes { get; }
	}
}