using System;

namespace Deveel.Workflows {
	public sealed class DefaultBuildContext : IBuildContext {
		public object Resolve(Type serviceType) {
			return Activator.CreateInstance(serviceType, true);
		}
	}
}