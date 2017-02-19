using System;
using System.Linq;

namespace Deveel.Workflows {
	public class ExecuteError {
		public ExecuteError(string message) 
			: this(message, new ExecuteError[0]) {
		}

		public ExecuteError(string message, ExecuteError[] innerErrors) {
			Message = message;
			InnerErrors = innerErrors;
		}

		public ExecuteError(Exception exception) {
			Message = exception.Message;

			if (exception.InnerException != null) {
				if (exception.InnerException is AggregateException) {
					InnerErrors = ((AggregateException) exception.InnerException).InnerExceptions.Select(CreateError).ToArray();
				} else {
					InnerErrors = new[] {CreateError(exception.InnerException)};
				}
			}
		}

		public string Message { get; }

		public ExecuteError[] InnerErrors { get; }

		protected virtual ExecuteError CreateError(Exception exception) {
			return new ExecuteError(exception);
		}
	}
}