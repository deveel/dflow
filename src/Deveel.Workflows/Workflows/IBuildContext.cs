using System;

namespace Deveel.Workflows {
	public interface IBuildContext {
		object Resolve(Type activityType);
	}
}