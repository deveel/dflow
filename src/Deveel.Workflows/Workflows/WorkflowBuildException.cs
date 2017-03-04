using System;

namespace Deveel.Workflows {
	public class WorkflowBuildException : Exception {
		public WorkflowBuildException(string message, Exception innerException)
			: base(message, innerException) {
		}

		public WorkflowBuildException(string message)
			: base(message) {
		}

		public WorkflowBuildException() {
		}
	}
}