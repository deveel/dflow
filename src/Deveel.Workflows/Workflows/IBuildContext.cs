using System;

namespace Deveel.Workflows {
	/// <summary>
	/// Provides a context for the resolution of services
	/// during the build process of a workflow
	/// </summary>
	public interface IBuildContext {
		/// <summary>
		/// Resolves the provided type to  an instance of an object
		/// </summary>
		/// <param name="serviceType">The type to resolve.</param>
		/// <returns>
		/// Returns an instance of and object resolved from the provided type.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="serviceType"/>
		/// is <c>null</c></exception>
		/// <exception cref="ServiceResolutionException">If the given <paramref name="serviceType"/>
		/// was not resolved in the current context because of an unknown error.</exception>
		object Resolve(Type serviceType);
	}
}