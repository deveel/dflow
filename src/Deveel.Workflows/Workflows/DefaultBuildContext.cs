using System;

namespace Deveel.Workflows {
	public sealed class DefaultBuildContext : IBuildContext {
		public IActivity ResolveActivity(Type activityType) {
			return Activator.CreateInstance(activityType) as IActivity;
		}
	}
}