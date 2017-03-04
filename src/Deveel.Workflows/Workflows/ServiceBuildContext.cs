using System;

namespace Deveel.Workflows {
	public class ServiceBuildContext : IBuildContext {
		private readonly IServiceProvider serviceProvider;

		public ServiceBuildContext(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}

		public object Resolve(Type serviceType) {
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));

			try {
				return serviceProvider.GetService(serviceType) as IActivity;
			} catch (Exception ex) {
				throw new ServiceResolutionException(serviceType, $"Could not resolve type '{serviceType}' because of an error", ex);
			}
		}
	}
}