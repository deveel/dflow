using System;

namespace Deveel.Workflows {
	public class ServiceResolutionException : WorkflowBuildException {
		public ServiceResolutionException(Type serviceType, string message, Exception innerException)
			: base(message, innerException) {
			ServiceType = serviceType;
		}

		public ServiceResolutionException(Type serviceType, string message)
			: base(message) {
			ServiceType = serviceType;
		}

		public Type ServiceType { get; }
	}
}