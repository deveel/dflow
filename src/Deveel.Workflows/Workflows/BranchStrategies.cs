using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public static class BranchStrategies {
		public static IBranchStrategy Parallel => new ParallelStrategy();

		public static IBranchStrategy Sequential => new SequentialStrategy();

		#region ParallelStrategy

		class ParallelStrategy : IBranchStrategy {
			public bool IsParallel => true;

			public async Task<State> ExecuteAsync(IBranch branch, State state, CancellationToken cancellationToken) {
				var tasks = branch.Activities.Select(x => x.ExecuteAsync(state, cancellationToken)).ToList();
				var states = await Task.WhenAll(tasks);

				return state.SetValue(states);
			}
		}

		#endregion

		#region SequentialStrategy

		class SequentialStrategy : IBranchStrategy {
			public bool IsParallel => false;

			public async Task<State> ExecuteAsync(IBranch branch, State state, CancellationToken cancellationToken) {
				State current = state;
				foreach (var activity in branch.Activities) {
					current = await activity.ExecuteAsync(current, cancellationToken);
				}

				return current;
			}
		}

		#endregion
	}
}