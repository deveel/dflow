using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Deveel.Workflows {
	class BranchBuilder : IBranchBuilder {
		public BranchBuilder() {
			Activities = new List<IActivityBuilder>();
			Strategy = BranchStrategies.Sequential;
		}

		private string Name { get; set; }

		private IBranchStrategy Strategy { get; set; }

		private List<IActivityBuilder> Activities { get; set; }

		private Func<State, bool> Decision { get; set; }

		public IBranchBuilder Named(string name) {
			Name = name;
			return this;
		}

		public IBranchBuilder With(IBranchStrategy strategy) {
			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy));

			Strategy = strategy;
			return this;
		}

		public IBranchBuilder Activity(Action<IActivityBuilder> activity) {
			var builder = new ActivityBuilder();
			activity(builder);

			Activities.Add(builder);
			return this;
		}

		public IBranchBuilder If(Func<State, bool> decision) {
			Decision = decision;

			return this;
		}

		public IBranchActivity Build(IBuildContext context) {
			if (Activities.Count == 0)
				throw new InvalidOperationException("At least one activity must be defined in a branch");

			var activities = Activities.Select(x => x.Build(context)).ToList();

			if (activities.Count == 1 &&
				activities[0] is MergeActivity)
				throw new InvalidOperationException();

			return new BranchActivity(Name, Decision, Strategy, activities);
		}
	}
}