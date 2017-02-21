using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface INamedActivityBuilder {
		IConditionalActivityBuilder If(Func<State, bool> decision);

		INamedActivityBuilder With(string key, object metadata);

		void Branch(Action<IBranchBuilder> branch);

		void Execute(Func<State, CancellationToken, Task<State>> execution);
	}
}