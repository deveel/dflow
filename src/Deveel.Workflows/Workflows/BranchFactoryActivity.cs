using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class BranchFactoryActivity : Activity, IBranchActivity {
		public BranchFactoryActivity(string name, Func<State, bool> decision, IEnumerable<IActivity> activities, IStateFactory stateFactory)
			: base(name, decision) {
			if (stateFactory == null)
				throw new ArgumentNullException(nameof(stateFactory));

			Activities = activities;
			this.stateFactory = stateFactory;
		}

		private readonly IStateFactory stateFactory;

		protected override State NextState(State previous) {
			return previous.NewBranch(this);
		}

		protected override async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			var states = stateFactory.CreateStates(state);
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