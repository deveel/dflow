using System;

namespace Deveel.Workflows {
	public class ActivityResolveException : ActivityBuildException {
		public ActivityResolveException(Type activityType, string message)
			: base(message) {
			ActivityType = activityType;
		}

		public ActivityResolveException(Type activityType, string message, Exception innerException)
			: base(message, innerException) {
			ActivityType = activityType;
		}

		public ActivityResolveException(Type activityType)
			: this(activityType, $"Could not resolve activity of type '{activityType}' in the current context.") {
		}

		public Type ActivityType { get; }
	}
}