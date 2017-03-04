using System;

namespace Deveel.Workflows {
	public class ActivityBuildException : WorkflowBuildException {
		public ActivityBuildException(string message, Exception innerException)
			: base(message, innerException) {
		}

		public ActivityBuildException(string message)
			: base(message) {
		}

		public ActivityBuildException() {
		}
	}
}