using System;
using System.Threading;

namespace Deveel.Workflows {
	public interface IActivityBuilder {
		INamedActivityBuilder Named(string name);

		void OfType(Type type);

		void Proxy(IActivity activity);

		void Branch(Action<IBranchBuilder> branch);

		IActivity Build(IBuildContext context);
	}
}