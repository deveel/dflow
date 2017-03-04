using System;

namespace Deveel.Workflows {
	public class WorkflowExecutionException : Exception {
		public WorkflowExecutionException(string message, Exception innerException)
			: base(message, innerException) {
		}

		public WorkflowExecutionException(string message)
			: base(message) {
		}

		public WorkflowExecutionException() {
		}
	}
}