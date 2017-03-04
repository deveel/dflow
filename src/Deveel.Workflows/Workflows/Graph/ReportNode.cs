using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Graph {
	public sealed class ReportNode {
		private readonly State state;

		internal ReportNode(State state) {
			this.state = state;
		}

		public string ComponentName => state.ExecutionInfo.Component.Name;

		public string ExecutionPath => state.PathString;

		public object InputValue => state.Previous?.Value;

		public object OutputValue => state.Value;

		public bool ChangedValue => state.HasNewValue;

		public DateTimeOffset StartedAt => state.ExecutionInfo.StartedAt;

		public DateTimeOffset EndedAt => state.ExecutionInfo.EndedAt.Value;

		public TimeSpan Elapsed => state.ExecutionInfo.Elapsed;

		public bool Executed => state.ExecutionInfo.Executed;

		public bool Failed => state.ExecutionInfo.Failed;

		public IEnumerable<ExecuteError> Errors => state.ExecutionInfo.Errors;

		public bool Branch => state.IsBranch;

		public IEnumerable<ReportNode> Nodes => 
			state.IsBranch ? state.AsBranch().States.Select(x => new ReportNode(x)) : new ReportNode[0];
	}
}