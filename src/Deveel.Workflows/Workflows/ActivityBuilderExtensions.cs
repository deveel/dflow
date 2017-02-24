using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public static class ActivityBuilderExtensions {
		public static IActivityFactorableBuilder OfType<TActivity>(this IActivityBuilder builder) where TActivity : class, IActivity {
			return builder.OfType(typeof(TActivity));
		}

		public static IActivityFactorableBuilder Merge(this IActivityBuilder builder, string name, IMergeStrategy strategy) {
			return builder.Proxy(new MergeActivity(name, strategy));
		}

		public static IActivityFactorableBuilder Repeat(this IActivityBuilder builder, string name, IRepeatDecisor decisor) {
			return builder.Proxy(new RepeatActivity(name, decisor));
		}

		public static IActivityFactorableBuilder Repeat(this IActivityBuilder builder, string name, Func<State, bool> decisor) {
			return builder.Proxy(new RepeatActivity(name, RepeatDecision.New(decisor)));
		}

		public static IActivityFactorableBuilder Execute(this INamedActivityBuilder builder, Func<State, Task<State>> execution) {
			return builder.Execute((state, token) => execution(state));
		}

		public static IActivityFactorableBuilder Execute(this INamedActivityBuilder builder, Func<State, State> execution) {
			return builder.Execute((state, token) => {
				var result = execution(state);
				return Task.FromResult(result);
			});
		}

		public static IActivityFactorableBuilder Execute(this IConditionalActivityBuilder builder, Func<State, Task<State>> execution) {
			return builder.Execute((state, token) => execution(state));
		}

		public static IActivityFactorableBuilder Execute(this IConditionalActivityBuilder builder, Func<State, State> execution) {
			return builder.Execute((state, token) => {
				var result = execution(state);
				return Task.FromResult(result);
			});
		}

		public static void AsFactory(this IActivityFactorableBuilder builder, Func<State, IEnumerable<State>> factory) {
			builder.AsFactory(StateFactories.New(factory));
		}
	}
}