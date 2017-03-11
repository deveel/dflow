using System;

using Microsoft.Extensions.DependencyInjection;

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
				return ActivatorUtilities.CreateInstance(serviceProvider, serviceType);
			} catch (Exception ex) {
				throw new ServiceResolutionException(serviceType, $"Could not resolve type '{serviceType}' because of an error", ex);
			}
		}
	}
}