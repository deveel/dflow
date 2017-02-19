using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public interface IBranch : IComponent {
		IBranchStrategy Strategy { get; }

		IEnumerable<IActivity> Activities { get; }
	}
}