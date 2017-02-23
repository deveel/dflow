using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class ActivityFactoryActivity : Activity {
		private readonly IStateFactory stateFactory;
		private readonly IActivity activity;

		public ActivityFactoryActivity(string name, Func<State, bool> decision, IActivity activity, IStateFactory stateFactory)
			: base(name, decision) {
			this.stateFactory = stateFactory;
			this.activity = activity;
		}

		protected override State NextState(State previous) {
			return previous.NewBranch(this);
		}

		protected override async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			var states = stateFactory.CreateStates(state);
			var tasks = states.Select(x => activity.ExecuteAsync(x, cancellationToken));
			var results = await Task.WhenAll(tasks);
			return state.SetValue(results);
		}
	}
}