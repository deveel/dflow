using System;

namespace Deveel.Workflows {
	public class ServiceBuildContext : IBuildContext {
		private readonly IServiceProvider serviceProvider;

		public ServiceBuildContext(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}

		public IActivity ResolveActivity(Type activityType) {
			try {
				var activity = serviceProvider.GetService(activityType) as IActivity;
				if (activity == null)
					throw new ActivityResolveException(activityType);

				return activity;
			} catch (Exception ex) {
				throw new ActivityResolveException(activityType, $"Could not resolve type '{activityType}' because of an error", ex);
			}
		}
	}
}