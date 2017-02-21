using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Graph {
	public sealed class ReportNode {
		private readonly State state;

		internal ReportNode(State state) {
			this.state = state;
		}

		public string ComponentName => state.StateInfo.Component.Name;

		public string ExecutionPath => state.PathString;

		public object InputValue => state.Previous?.Value;

		public object OutputValue => state.Value;

		public bool ChangedValue => state.HasNewValue;

		public DateTimeOffset StartedAt => state.StateInfo.StartedAt;

		public DateTimeOffset EndedAt => state.StateInfo.EndedAt.Value;

		public TimeSpan Elapsed => state.StateInfo.Elapsed;

		public bool Executed => state.StateInfo.Executed;

		public bool Failed => state.StateInfo.Failed;

		public IEnumerable<ExecuteError> Errors => state.StateInfo.Errors;

		public bool Branch => state.IsBranch;

		public IEnumerable<ReportNode> Nodes => 
			state.IsBranch ? state.AsBranch().States.Select(x => new ReportNode(x)) : new ReportNode[0];
	}
}