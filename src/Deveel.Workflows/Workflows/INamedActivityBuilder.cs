using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface INamedActivityBuilder {
		IConditionalActivityBuilder If(Func<State, bool> decision);

		void Branch(Action<IBranchBuilder> branch);

		void Execute(Func<State, CancellationToken, Task<State>> execution);
	}
}