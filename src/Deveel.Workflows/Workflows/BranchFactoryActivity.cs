using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class BranchFactoryActivity : Activity, IBranchActivity {
		private Func<State, bool> decision;

		public BranchFactoryActivity(string name, Func<State, bool> decision, IEnumerable<IActivity> activities, IStateFactory stateFactory)
			: base(name) {
			if (stateFactory == null)
				throw new ArgumentNullException(nameof(stateFactory));

			Activities = activities;
			StateFactory = stateFactory;
			this.decision = decision;
		}

		public IStateFactory StateFactory { get; }

		public override bool HasDecision => decision != null;

		protected override bool CanExecute(State state) {
			return decision?.Invoke(state) ?? base.CanExecute(state);
		}

		protected override State NextState(State previous) {
			return previous.NewBranch(this);
		}

		protected override async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			var states = StateFactory.CreateStates(state);
			var tasks = states.Select(x => BranchStrategies.Parallel.ExecuteAsync(this, x, cancellationToken));
			var results = await Task.WhenAll(tasks);
			return state.SetValue(results);
		}

		IBranchStrategy IBranch.Strategy {
			get { return BranchStrategies.Parallel; }
		}

		public IEnumerable<IActivity> Activities { get; }
	}
}