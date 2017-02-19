using System;

namespace Deveel.Workflows {
	public static class RepeatDecision {
		public static IRepeatDecisor New(Func<State, bool> decisor) {
			return new DelegatedRepeatDecisor(decisor);
		}

		#region DelegatedRepeatStrategy

		class DelegatedRepeatDecisor : IRepeatDecisor {
			private Func<State, bool> decision;

			public DelegatedRepeatDecisor(Func<State, bool> decision) {
				this.decision = decision;
			}

			public bool Decide(State state) {
				return decision(state);
			}
		}

		#endregion
	}
}