using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public static class StateFactories {
		public static IStateFactory New(Func<State, IEnumerable<State>> stateFactory) {
			return new DelegatedStateFactory(stateFactory);
		}

		#region DelegatedStateFactory

		class DelegatedStateFactory : IStateFactory {
			private Func<State, IEnumerable<State>> factory;

			public DelegatedStateFactory(Func<State, IEnumerable<State>> factory) {
				this.factory = factory;
			}

			public IEnumerable<State> CreateStates(State state) {
				return factory(state);
			}
		}

		#endregion
	}
}