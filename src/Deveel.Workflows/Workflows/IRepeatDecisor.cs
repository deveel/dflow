using System;

namespace Deveel.Workflows {
	public interface IRepeatDecisor {
		bool Decide(State state);
	}
}