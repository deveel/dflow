using System;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public static class ActivityBuilderExtensions {
		public static void Merge(this IActivityBuilder builder, string name, IMergeStrategy strategy) {
			builder.Proxy(new MergeActivity(name, strategy));
		}

		public static void Repeat(this IActivityBuilder builder, string name, IRepeatDecisor decisor) {
			builder.Proxy(new RepeatActivity(name, decisor));
		}

		public static void Repeat(this IActivityBuilder builder, string name, Func<State, bool> decisor) {
			builder.Proxy(new RepeatActivity(name, RepeatDecision.New(decisor)));
		}

		public static void Execute(this INamedActivityBuilder builder, Func<State, Task<State>> execution) {
			builder.Execute((state, token) => execution(state));
		}

		public static void Execute(this INamedActivityBuilder builder, Func<State, State> execution) {
			builder.Execute((state, token) => {
				var result = execution(state);
				return Task.FromResult(result);
			});
		}

		public static void Execute(this IConditionalActivityBuilder builder, Func<State, Task<State>> execution) {
			builder.Execute((state, token) => execution(state));
		}

		public static void Execute(this IConditionalActivityBuilder builder, Func<State, State> execution) {
			builder.Execute((state, token) => {
				var result = execution(state);
				return Task.FromResult(result);
			});
		}

	}
}