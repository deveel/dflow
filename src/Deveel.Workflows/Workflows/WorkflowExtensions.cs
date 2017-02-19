using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public static class WorkflowExtensions {
		public static Task<State> ExecuteAsync(this IWorkflow workflow, State state, CancellationToken cancellationToken) {
			return workflow.Strategy.ExecuteAsync(workflow, state, cancellationToken);
		}

		public static Task<State> ExecuteAsync(this IWorkflow workflow, State state) {
			return workflow.ExecuteAsync(state, CancellationToken.None);
		}

		public static Task<State> ExecuteAsync(this IWorkflow workflow, CancellationToken cancellationToken) {
			return workflow.ExecuteAsync(new State(), cancellationToken);
		}

		public static Task<State> ExecuteAsync(this IWorkflow workflow) {
			return workflow.ExecuteAsync(new State());
		}

		public static Task<TOutput> ExecuteAsync<TInput, TOutput>(this IWorkflow workflow, TInput input) {
			return ExecuteAsync<TInput, TOutput>(workflow, input, CancellationToken.None);
		}

		public static async Task<TOutput> ExecuteAsync<TInput, TOutput>(this IWorkflow workflow, TInput input, CancellationToken cancellationToken) {
			var result = await workflow.ExecuteAsync(new State(input), cancellationToken);
			return (TOutput) result.Value;
		}

		public static Task<T> ExecuteAsync<T>(this IWorkflow workflow, T input) {
			return ExecuteAsync<T>(workflow, input, CancellationToken.None);
		}

		public static Task<T> ExecuteAsync<T>(this IWorkflow workflow, T input, CancellationToken cancellationToken) {
			return workflow.ExecuteAsync<T, T>(input, cancellationToken);
		}

		public static State Execute(this IWorkflow workflow, State state) {
			return workflow.ExecuteAsync(state).Result;
		}

		public static State Execute(this IWorkflow workflow) {
			return workflow.Execute(new State());
		}

		public static TOutput Execute<TInput, TOutput>(this IWorkflow workflow, TInput input) {
			return workflow.ExecuteAsync<TInput, TOutput>(input, CancellationToken.None).Result;
		}

		public static T Execute<T>(this IWorkflow workflow, T input) {
			return workflow.Execute<T, T>(input);
		}
	}
}