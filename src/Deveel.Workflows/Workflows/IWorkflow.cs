using System;

namespace Deveel.Workflows {
	/// <summary>
	/// The container of components (sub-branches or activities) to 
	/// be executed given an input state until an exit point.
	/// </summary>
	/// <remarks>
	/// Workflows are conceptually designed as <see cref="IBranch"/>
	/// and are executed with a sequential strategy.
	/// </remarks>
	/// <seealso cref="IBranch"/>
	public interface IWorkflow : IBranch {
	}
}