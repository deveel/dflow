using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class MergeActivity : Activity {
		public MergeActivity(string name, IMergeStrategy strategy)
			: base(name) {
			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy));

			Strategy = strategy;
		}

		public IMergeStrategy Strategy { get; }

		protected override Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			var branchState = state.Previous as ParallelState;
			if (branchState == null)
				throw new NotSupportedException("The previous state was not of a branch activity");

			var result = branchState.Merge(Strategy);
			state.SetValue(result);
			return Task.FromResult(state);
		}
	}
}