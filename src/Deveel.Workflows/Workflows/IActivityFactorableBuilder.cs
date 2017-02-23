using System;

namespace Deveel.Workflows {
	public interface IActivityFactorableBuilder {
		void AsFactory(IStateFactory stateFactory);
	}
}