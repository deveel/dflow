using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public static class BranchBuilderExtensions {
		public static IActivityBranchBuilder InParallel(this IActivityBranchBuilder builder) {
			return builder.With(BranchStrategies.Parallel);
		}

		public static IActivityBranchBuilder InSequence(this IActivityBranchBuilder builder) {
			return builder.With(BranchStrategies.Sequential);
		}

		public static IActivityBranchBuilder Activity<TActivity>(this IActivityBranchBuilder builder)
			where TActivity : class, IActivity {
			return builder.Activity(activity => activity.OfType(typeof(TActivity)));
		}

		public static IActivityBranchBuilder Activity<TActivity>(this IActivityBranchBuilder builder, TActivity activity)
			where TActivity : class, IActivity {
			return builder.Activity(x => x.Proxy(activity));
		}

		public static IActivityBranchBuilder Branch(this IActivityBranchBuilder builder, Action<IActivityBranchBuilder> branch) {
			return builder.Activity(activity => activity.Branch(branch));
		}

		public static IActivityBranchBuilder Merge(this IActivityBranchBuilder builder, string name, IMergeStrategy strategy) {
			return builder.Activity(activity => activity.Merge(name, strategy));
		}

		public static IActivityBranchBuilder Merge(this IActivityBranchBuilder builder, string name, Func<IEnumerable<object>, object> merge) {
			return builder.Merge(name, MergeStrategies.New(merge));
		}

		public static IActivityBranchBuilder Repeat(this IActivityBranchBuilder builder, string name, IRepeatDecisor decisor) {
			return builder.Activity(activity => activity.Repeat(name, decisor));
		}

		public static IActivityBranchBuilder Repeat(this IActivityBranchBuilder builder, string name, Func<State, bool> decisor) {
			return builder.Activity(activity => activity.Repeat(name, RepeatDecision.New(decisor)));
		}

		public static void AsFactory(this IActivityBranchBuilder builder, Func<State, IEnumerable<State>> stateFactory) {
			builder.AsFactory(StateFactories.New(stateFactory));
		}
	}
}