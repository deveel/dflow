using System;

namespace Deveel.Workflows {
	public interface IBuildContext {
		IActivity ResolveActivity(Type activityType);
	}
}