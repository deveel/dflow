using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	/// <summary>
	/// An activity in an execution strategy of a workflow
	/// </summary>
	/// <remarks>
	/// Activities are components of a workflow and do not carry
	/// any state, but execute the provided state from previous
	/// activities or the initiating point.
	/// </remarks>
	/// <seealso cref="IWorkflow"/>
	/// <seealso cref="IComponent"/>
	public interface IActivity : IComponent {
		/// <summary>
		/// Gets a set of key/value pair that describe the metadata
		/// of the activity (eg. name, returned type, description, etc.)
		/// </summary>
		/// <remarks>
		/// Implementations of <see cref="IActivity"/> can implement
		/// methods for returning the metadata directly or use provided
		/// attributes for description
		/// </remarks>
		/// <seealso cref="KnownActivityMetadataKeys"/>
		IEnumerable<KeyValuePair<string, object>> Metadata { get; }

		/// <summary>
		/// Gets a flag state indicating if the activity specifies a
		/// decision logic to activate the execution
		/// </summary>
		/// <remarks>
		/// Implementations of <see cref="IActivity"/> can implement
		/// a check on the incoming state to determine if the state
		/// incoming can trigger the execution of the activity
		/// </remarks>
		bool HasDecision { get; }

		/// <summary>
		/// Executes the routines defined by the activity against the
		/// provided input state, returning a result state
		/// </summary>
		/// <param name="state">The input state provided from the previous
		/// activities of the initiating point.</param>
		/// <param name="cancellationToken">A token that controls the parallelism
		/// of the execution</param>
		/// <returns>
		/// Returns a new instance of <see cref="State"/> that represents the results
		/// of the execution of the activity.
		/// </returns>
		/// <remarks>
		/// <para>
		/// A new state should always be generated from an activity, even when not transforming
		/// the input provided, to allow a later analysis of the execution graph
		/// </para>
		/// <para>
		/// The execution of an activity should not throw any exception to the extern,
		/// rather register the error in the state information. If the execution must
		/// be imperatively stopped because of a fatal error, the implementation must throw 
		/// an instance of <see cref="ActivityExecutionException"/>. In any case, the
		/// containing context of the activity should catch an wrap any non-handled execution
		/// thrown by the execution of an activity into a <see cref="ActivityExecutionException"/>.
		/// </para>
		/// <para>
		/// Information on the execution of the activity can be registered into the instance
		/// of <see cref="ExecutionInfo"/> provided by the <see cref="State"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="State"/>
		/// <seealso cref="ExecutionInfo"/>
		/// <exception cref="ActivityExecutionException">If any fatal exception occurred that must
		/// complete the execution of the workflow.</exception>
		Task<State> ExecuteAsync(State state, CancellationToken cancellationToken);
	}
}