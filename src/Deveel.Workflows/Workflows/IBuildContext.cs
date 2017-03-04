using System;

namespace Deveel.Workflows {
	/// <summary>
	/// Provides a context for the resolution of activities
	/// during the build process of a workflow
	/// </summary>
	public interface IBuildContext {
		/// <summary>
		/// Resolves the provided type to  an instance of
		/// <see cref="IActivity"/>
		/// </summary>
		/// <param name="activityType">The type to resolve.</param>
		/// <returns>
		/// Returns an instance of <see cref="IActivity"/> resolved
		/// from the provided type.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="activityType"/>
		/// is <c>null</c></exception>
		/// <exception cref="ArgumentException">If the provided <paramref name="activityType"/>
		/// is not assignable from <see cref="IActivity"/>.</exception>
		/// <exception cref="ActivityResolveException">If it was not possible to resolve
		/// the activity because of an unknown error.</exception>
		IActivity ResolveActivity(Type activityType);
	}
}