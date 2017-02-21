using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Graph {
	public sealed class ExecutionReport {
		private readonly List<ReportNode> nodes;

		private ExecutionReport(IEnumerable<ReportNode> nodes) {
			this.nodes = new List<ReportNode>(nodes);
		}

		public IEnumerable<ReportNode> Nodes => nodes.AsReadOnly();

		public static ExecutionReport Build(State finalState) {
			var states = new List<State>();
			State current = finalState;
			while (current != null) {
				states.Add(current);

				if (current.IsBranch) {
					foreach (var inner in current.AsBranch().States) {
						states.Add(inner);
					}
				}

				current = current.Previous;
			}

			states.Reverse();

			return new ExecutionReport(states.Select(x => new ReportNode(x)));
		}
	}
}