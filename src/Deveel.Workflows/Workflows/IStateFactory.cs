using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public interface IStateFactory {
		IEnumerable<State> CreateStates(State state);
	}
}