using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows {
	public sealed class ParallelState : State {
		internal ParallelState(State previous, IComponent component)
			: base(previous, component) {
		}

		public new object[] Value =>States.Select(x => x.Value).ToArray();

		public IEnumerable<State> States => (IEnumerable<State>) base.Value;

		public object Merge(IMergeStrategy strategy) {
			return strategy.Merge(Value);
		}
	}
}