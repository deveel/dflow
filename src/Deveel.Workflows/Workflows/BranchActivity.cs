using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class BranchActivity : Activity, IBranchActivity {
		private Func<State, bool> decision;

		public BranchActivity(IBranchStrategy strategy, IEnumerable<IActivity> activities) 
			: this((string)null, strategy, activities) {
		}

		public BranchActivity(string name, IBranchStrategy strategy, IEnumerable<IActivity> activities) 
			: this(name, null, strategy, activities) {
		}

		public BranchActivity(Func<State, bool> decision, IBranchStrategy strategy, IEnumerable<IActivity> activities)
			: this(null, decision, strategy, activities) {
		}

		public BranchActivity(string name, Func<State, bool> decision, IBranchStrategy strategy, IEnumerable<IActivity> activities)
			: base(name) {
			this.decision = decision;
			Strategy = strategy;
			Activities = activities;
		}

		public IBranchStrategy Strategy { get; }

		public IEnumerable<IActivity> Activities { get; }

		protected override bool CanExecute(State state) {
			return decision?.Invoke(state) ?? true;
		}

		protected override State NextState(State previous) {
			if (Strategy.IsParallel)
				return previous.GetNextParallel(this);

			return base.NextState(previous);
		}

		protected override Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			return Strategy.ExecuteAsync(this, state, cancellationToken);
		}
	}
}