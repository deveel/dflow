using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public interface IMergeStrategy {
		object Merge(IEnumerable<object> objects);
	}
}