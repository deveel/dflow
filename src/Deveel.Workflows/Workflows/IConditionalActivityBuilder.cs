using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface IConditionalActivityBuilder {
		void Branch(Action<IActivityBranchBuilder> branch);

		IActivityFactorableBuilder Execute(Func<State, CancellationToken, Task<State>> execution);
	}
}