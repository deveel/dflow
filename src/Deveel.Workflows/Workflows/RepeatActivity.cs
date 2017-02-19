using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public sealed class RepeatActivity : Activity {
		public RepeatActivity(string name, IRepeatDecisor decisor)
			: base(name) {
			Decisor = decisor;
		}

		public IRepeatDecisor  Decisor { get; }

		protected override bool CanExecute(State state) {
			return Decisor.Decide(state);
		}

		protected override async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			IBranch branch = null;

			State current = state;
			while (current != null) {
				branch = current.StateInfo.Component as IBranch;
				if (branch != null)
					break;

				current = current.Previous;
			}

			if (branch == null)
				throw new InvalidOperationException();

			var result = await branch.Strategy.ExecuteAsync(branch, state, cancellationToken);
			while (CanExecute(result)) {
				result = await branch.Strategy.ExecuteAsync(branch, result, cancellationToken);
			}

			return result;
		}
	}
}