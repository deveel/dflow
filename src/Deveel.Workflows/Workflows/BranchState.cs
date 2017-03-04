using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows {
	public sealed class BranchState : State {
		internal BranchState(State previous, IComponent component)
			: base(previous, component) {
		}

		public new object[] Value => States.Select(x => x.Value).ToArray();

		public IEnumerable<State> States => (IEnumerable<State>) base.Value;

		public bool IsParallel => ((IBranch) ExecutionInfo.Component).Strategy.IsParallel;

		public object Merge(IMergeStrategy strategy) {
			return strategy.Merge(Value);
		}
	}
}