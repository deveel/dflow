using System;

namespace Deveel.Workflows {
	/// <summary>
	/// Handles a registry of workflows in a context
	/// </summary>
	/// <remarks>
	/// Implementations of registries define workflow 
	/// registration and resolve logic
	/// </remarks>
	public interface IWorkflowRegistry {
		/// <summary>
		/// Retrieves a workflow registered in the
		/// registry for the given name.
		/// </summary>
		/// <param name="name">The name of the workflow to
		/// return from the registry.</param>
		/// <returns>
		/// Returns an instance of <see cref="IWorkflow"/> that
		/// is associated to the given <paramref name="name"/>, or
		/// <c>null</c> if none was found for the given name.
		/// </returns>
		IWorkflow GetWorkflow(string name);
	}
}