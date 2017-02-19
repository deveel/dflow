using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows {
	public static class MergeStrategies {
		public static IMergeStrategy New(Func<IEnumerable<object>, object> merge) {
			return new DelegatedMergeStrategy(merge);
		}

		public static IMergeStrategy New<TInput, TOutput>(Func<IEnumerable<TInput>, TOutput> merge) {
			return new TypedDelegatedMergeStrategy<TInput, TOutput>(merge);
		}

		#region DelegatedMergeStrategy

		class DelegatedMergeStrategy : IMergeStrategy {
			private readonly Func<IEnumerable<object>, object> merge;

			public DelegatedMergeStrategy(Func<IEnumerable<object>, object> merge) {
				this.merge = merge;
			}

			public object Merge(IEnumerable<object> objects) {
				return merge(objects);
			}
		}

		#endregion

		#region TypedDelegatedMergeStrategy

		class TypedDelegatedMergeStrategy<TInput, TOutput> : IMergeStrategy {
			private Func<IEnumerable<TInput>, TOutput> merge;

			public TypedDelegatedMergeStrategy(Func<IEnumerable<TInput>, TOutput> merge) {
				this.merge = merge;
			}

			public object Merge(IEnumerable<object> objects) {
				var mapped = objects.Cast<TInput>();
				return merge(mapped);
			}
		}

		#endregion
	}
}