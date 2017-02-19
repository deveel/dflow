using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public static class BranchBuilderExtensions {
		public static IBranchBuilder InParallel(this IBranchBuilder builder) {
			return builder.With(BranchStrategies.Parallel);
		}

		public static IBranchBuilder InSequence(this IBranchBuilder builder) {
			return builder.With(BranchStrategies.Sequential);
		}

		public static IBranchBuilder Activity<TActivity>(this IBranchBuilder builder)
			where TActivity : class, IActivity {
			return builder.Activity(activity => activity.OfType(typeof(TActivity)));
		}

		public static IBranchBuilder Activity<TActivity>(this IBranchBuilder builder, TActivity activity)
			where TActivity : class, IActivity {
			return builder.Activity(x => x.Proxy(activity));
		}

		public static IBranchBuilder Merge(this IBranchBuilder builder, string name, IMergeStrategy strategy) {
			return builder.Activity(activity => activity.Merge(name, strategy));
		}

		public static IBranchBuilder Merge(this IBranchBuilder builder, string name, Func<IEnumerable<object>, object> merge) {
			return builder.Merge(name, MergeStrategies.New(merge));
		}

		public static IBranchBuilder Repeat(this IBranchBuilder builder, string name, IRepeatDecisor decisor) {
			return builder.Activity(activity => activity.Repeat(name, decisor));
		}

		public static IBranchBuilder Repeat(this IBranchBuilder builder, string name, Func<State, bool> decisor) {
			return builder.Activity(activity => activity.Repeat(name, RepeatDecision.New(decisor)));
		}
	}
}