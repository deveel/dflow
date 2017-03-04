using System;
using System.Threading;

namespace Deveel.Workflows {
	/// <summary>
	/// Provides utilities for a delayed building an instance of
	/// <seealso cref="IActivity"/>
	/// </summary>
	/// <remarks>
	/// Activity builders provide design logic for shaping the
	/// properties of an activity: the build of the <see cref="IActivity"/>
	/// is done within a given <see cref="IBuildContext"/>, typically
	/// invoked by parent workflow builder, before the execution
	/// of the workflow.
	/// </remarks>
	public interface IActivityBuilder {
		/// <summary>
		/// Specifies a name for the activity
		/// </summary>
		/// <param name="name">The name of the activity</param>
		/// <returns>
		/// Returns this builder after the configuration of the property.
		/// </returns>
		/// <remarks>
		/// The name of an activity is required only in case of built
		/// activities: when using references (through <see cref="OfType"/>
		/// of <see cref="Proxy"/>) this is not required.
		/// </remarks>
		INamedActivityBuilder Named(string name);

		/// <summary>
		/// Indicates the type of an implementing activity to
		/// reference and that will be resolved at build.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the activity to reference</param>
		/// <returns>
		/// Returns an instance of a <see cref="IActivityFactorableBuilder"/> that will
		/// allow specifying if this configuration will be used as a factory.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the passed <paramref name="type"/>
		/// is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">If the <paramref name="type"/> is not
		/// assignable from <see cref="IActivity"/> or if it is not instantiable (either
		/// an interface or an abstract class)</exception>
		/// <seealso cref="IActivityFactorableBuilder"/>
		IActivityFactorableBuilder OfType(Type type);

		/// <summary>
		/// Provides an instance of <see cref="IActivity"/> that will be invoked
		/// during the execution of the work-flow
		/// </summary>
		/// <param name="activity">The instance of the activity to be proxied</param>
		/// <returns>
		/// Returns an instance of a <see cref="IActivityFactorableBuilder"/> that will
		/// allow specifying if this configuration will be used as a factory.
		/// </returns>
		/// <remarks>
		/// Attention must be put on the handling of the provided instance, since the
		/// execution context and the life-time of the instance could be not aligned:
		/// if the provided instance will be disposed or not available anymore during
		/// the execution of the context this will generate a fatal exception.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="activity"/>
		/// is <c>null</c>.</exception>
		IActivityFactorableBuilder Proxy(IActivity activity);

		/// <summary>
		/// Creates a branched activity.
		/// </summary>
		/// <param name="branch">The builder used to configure the branched activity.</param>
		/// <seealso cref="IActivityBranchBuilder"/>
		void Branch(Action<IActivityBranchBuilder> branch);

		/// <summary>
		/// Constructs an instance of <see cref="IActivity"/> from the
		/// configured properties of the builder, against the provided
		/// resolution context.
		/// </summary>
		/// <param name="context">The context used to resolve the external
		/// reference to activities. If the builder has no external
		/// references to activities, the context is not strictly necessary.</param>
		/// <returns>
		/// Returns an instance of <see cref="IActivity"/> as the result
		/// of the configurations specified in this builder.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the builder has any reference to
		/// activity types, and the <paramref name="context"/> provided is<c>null</c>.</exception>
		/// <exception cref="ActivityBuildException">If it was not possible to build
		/// the activity instance because of an non handled error.</exception>
		/// <exception cref="ActivityResolveException">If it was not possible to resolve
		/// a type of activity within the specified context.</exception>
		/// <seealso cref="IBuildContext"/>
		IActivity Build(IBuildContext context);
	}
}