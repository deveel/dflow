using System;

namespace Deveel.Workflows {
	public class ActivityExecutionException : WorkflowExecutionException {
		public ActivityExecutionException(string message, Exception innerException)
			: base(message, innerException) {
		}

		public ActivityExecutionException(string message)
			: base(message) {
		}

		public ActivityExecutionException() {
		}
	}
}