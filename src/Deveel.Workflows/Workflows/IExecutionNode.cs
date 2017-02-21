using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public interface IExecutionNode {
		string Name { get; }

		bool HasDecision { get; }

		IExecutionNode Previous { get; }

		IExecutionNode Next { get; }

		IDictionary<string, object> Metadata { get; }

		IEnumerable<IExecutionNode> Nodes { get; }
	}
}