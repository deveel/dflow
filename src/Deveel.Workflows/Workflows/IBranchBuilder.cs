using System;

namespace Deveel.Workflows {
	public interface IBranchBuilder {
		IBranchBuilder Named(string name);

		IBranchBuilder With(IBranchStrategy strategy);

		IBranchBuilder If(Func<State, bool> decision);

		IBranchBuilder Activity(Action<IActivityBuilder> activity);

		void AsFactory(IStateFactory stateFactory);

		void AsFactory(Type factoryType);

		IBranchActivity Build(IBuildContext context);
	}
}