using System;

namespace Deveel.Workflows {
	/// <summary>
	/// An utility to configure the properties of a branch activity
	/// </summary>
	public interface IActivityBranchBuilder {
		/// <summary>
		/// Sets the name of the branch
		/// </summary>
		/// <param name="name">The name of the branch</param>
		/// <returns>
		/// Returns an instance of this builder with the configured 
		/// property.
		/// </returns>
		/// <exception cref="ArgumentNullException">If the provided
		/// <paramref name="name"/> is <c>null</c> or empty.</exception>
		IActivityBranchBuilder Named(string name);

		/// <summary>
		/// Sets the strategy of execution of the activities in 
		/// the resulting branch
		/// </summary>
		/// <param name="strategy">The execution strategy to be used
		/// during the execution of the branch resulting from the build.</param>
		/// <returns>
		/// Returns an instance of this builder with the configured 
		/// property.
		/// </returns>
		/// <remarks>
		/// <para>
		/// An execution strategy is required to build a branched activity:
		/// by default a builder sets it to <see cref="BranchStrategies.Sequential"/>,
		/// if none is set through this method.
		/// </para>
		/// <para>
		/// Attention must be paid to the lifetime of the instance of
		/// <see cref="IBranchStrategy"/> passed: although the typical
		/// implementation is static, if the strategy specified in the builder
		/// is later disposed or not available during the execution of the
		/// branch resulted from the build of this builder, a fatal exception
		/// will occur that will cause the failure of the execution.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="strategy"/>
		/// is <c>null</c>.</exception>
		IActivityBranchBuilder With(IBranchStrategy strategy);

		/// <summary>
		/// Sets a condition to be evaluated to activate the execution
		/// of the branch.
		/// </summary>
		/// <param name="decision">The function that is evaluated before
		/// executing the resulting branch activity, to determine if the
		/// execution must occur.</param>
		/// <returns>
		/// Returns an instance of this builder with the configured 
		/// property.
		/// </returns>
		IActivityBranchBuilder If(Func<State, bool> decision);

		/// <summary>
		/// Configures a contained activity in the resulting branch.
		/// </summary>
		/// <param name="activity">The builder used to configure the activity to
		/// be built and contained into the resulting branch.</param>
		/// <returns>
		/// Returns an instance of this builder with the configured 
		/// property.
		/// </returns>
		IActivityBranchBuilder Activity(Action<IActivityBuilder> activity);

		/// <summary>
		/// Indicates that the properties set in this builder
		/// will be used to construct a factory of branches, given the
		/// factory strategy provided.
		/// </summary>
		/// <param name="stateFactory">The instance of <see cref="IStateFactory"/> that
		/// is used to generate instances of branch activities at run-time, using
		/// the configuration provided by this builder</param>
		/// <seealso cref="IStateFactory"/>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="stateFactory"/>
		/// is <c>null</c>.</exception>
		void AsFactory(IStateFactory stateFactory);

		void AsFactory(Type factoryType);

		/// <summary>
		/// Builds an instance of <see cref="IBranchActivity"/> using the
		/// configured properties in this builder, against a provided
		/// context for resolving references.
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
		/// <seealso cref="IActivityBuilder"/>
		IBranchActivity Build(IBuildContext context);
	}
}