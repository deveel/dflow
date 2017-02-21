using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Graph {
	class BuilderNode : ExecutionNode {
		public BuilderNode(string name, IEnumerable<KeyValuePair<string, object>> metadata) 
			: this(name, false, metadata) {
		}

		public BuilderNode(string name, bool decision, IEnumerable<KeyValuePair<string, object>> metadata) {
			Name = name;
			HasDecision = decision;

			Metadata = new Dictionary<string, object>();

			if (metadata != null) {
				foreach (var pair in metadata) {
					Metadata[pair.Key] = pair.Value;
				}
			}
		}

		public override string Name { get; }

		public override IDictionary<string, object> Metadata { get; }

		public override bool HasDecision { get; }

		public static IEnumerable<IExecutionNode> BuildChain(IEnumerable<ExecutionNode> nodes) {
			var list = nodes.ToList();
			return list.Select((node, offset) => {
				var next = offset + 1 >= list.Count ? null : list[offset + 1];
				var previous = offset - 1 < 0 ? null : list[offset - 1];

				if (next != null)
					node.SetNext(next);
				if (previous != null)
					node.SetPrevious(previous);

				return node;
			});

		}
	}
}