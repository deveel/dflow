using System;

namespace Deveel.Workflows {
	public sealed class DefaultBuildContext : IBuildContext {
		public object Resolve(Type serviceType) {
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));

			try {
				return Activator.CreateInstance(serviceType, true);
			} catch (Exception ex) {
				throw new ServiceResolutionException(serviceType, $"Could not resolve service of type {serviceType} because of an error", ex);
			}
		}
	}
}