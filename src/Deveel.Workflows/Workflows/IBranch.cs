using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	/// <summary>
	/// A container of activities that are executed
	/// according to the provided strategy.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Workflows are the main branch of execution and they
	/// define a sequential strategy of execution of the contained
	/// activities.
	/// </para>
	/// </remarks>
	/// <seealso cref="IBranchStrategy"/>
	public interface IBranch : IComponent {
		/// <summary>
		/// Gets the instance of the <see cref="IBranchStrategy"/> used
		/// to execute the contained activities
		/// </summary>
		/// <seealso cref="IBranchStrategy"/>
		IBranchStrategy Strategy { get; }

		/// <summary>
		/// Gets an enumeration of the <see cref="IActivity"/> objects
		/// contained by this branch and that will be executed accordingly
		/// to the strategy defined.
		/// </summary>
		/// <seealso cref="IActivity"/>
		IEnumerable<IActivity> Activities { get; }
	}
}