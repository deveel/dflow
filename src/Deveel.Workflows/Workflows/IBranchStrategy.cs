using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface IBranchStrategy {
		bool IsParallel { get; }

		Task<State> ExecuteAsync(IBranch branch, State state, CancellationToken cancellationToken);
	}
}