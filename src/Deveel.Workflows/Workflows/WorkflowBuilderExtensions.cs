using System;
using System.Collections.Generic;

namespace Deveel.Workflows {
	public static class WorkflowBuilderExtensions {
		public static IWorkflowBuilder Activity<TActivity>(this IWorkflowBuilder builder)
			where TActivity : class, IActivity {
			return builder.Activity(activity => activity.OfType(typeof(TActivity)));
		}

		public static IWorkflowBuilder Activity<TActivity>(this IWorkflowBuilder builder, TActivity activity)
			where TActivity : class, IActivity {
			return builder.Activity(x => x.Proxy(activity));
		}

		public static IWorkflowBuilder Branch(this IWorkflowBuilder builder, Action<IBranchBuilder> branch) {
			return builder.Activity(activity => activity.Branch(branch));
		}

		public static IWorkflowBuilder Merge(this IWorkflowBuilder builder, string name, IMergeStrategy strategy) {
			return builder.Activity(activity => activity.Merge(name, strategy));
		}

		public static IWorkflowBuilder Merge(this IWorkflowBuilder builder, string name, Func<IEnumerable<object>, object> merge) {
			return builder.Merge(name, MergeStrategies.New(merge));
		}

		public static IWorkflowBuilder Repeat(this IWorkflowBuilder builder, string name, IRepeatDecisor decisor) {
			return builder.Activity(activity => activity.Repeat(name, decisor));
		}

		public static IWorkflowBuilder Repeat(this IWorkflowBuilder builder, string name, Func<State, bool> decisor) {
			return builder.Activity(activity => activity.Repeat(name, decisor));
		}

		public static IWorkflow Build(this IWorkflowBuilder builder) {
			return builder.Build(new DefaultBuildContext());
		}
	}
}