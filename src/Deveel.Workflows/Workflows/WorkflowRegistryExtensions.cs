using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public static class WorkflowRegistryExtensions {
		public static Task<State> ExecuteAsync(this IWorkflowRegistry registry, string name, State state, CancellationToken cancellationToken) {
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			var workflow = registry.GetWorkflow(name);
			if (workflow == null)
				throw new WorkflowExecutionException($"No workflow named '{name}' was specified in the registry");

			return workflow.ExecuteAsync(state, cancellationToken);
		}

		public static Task<State> ExecuteAsync(this IWorkflowRegistry registry, string name, State state)
			=> registry.ExecuteAsync(name, state, CancellationToken.None);
	}
}