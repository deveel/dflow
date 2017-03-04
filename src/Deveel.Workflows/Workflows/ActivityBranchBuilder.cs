using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Deveel.Workflows.Graph;

namespace Deveel.Workflows {
	class ActivityBranchBuilder : IActivityBranchBuilder, IExecutionNodeBuilder {
		public ActivityBranchBuilder() {
			Activities = new List<ActivityBuilder>();
			Strategy = BranchStrategies.Sequential;
		}

		private string Name { get; set; }

		private IBranchStrategy Strategy { get; set; }

		private List<ActivityBuilder> Activities { get; set; }

		private Func<State, bool> Decision { get; set; }

		private IDictionary<string, object> Metadata { get; set; }

		private bool Factory { get; set; }

		private IStateFactory StateFactory { get; set; }

		private Type StateFactoryType { get; set; }

		public IActivityBranchBuilder Named(string name) {
			Name = name;
			return this;
		}

		public IActivityBranchBuilder With(IBranchStrategy strategy) {
			if (strategy == null)
				throw new ArgumentNullException(nameof(strategy));

			Strategy = strategy;
			return this;
		}

		public IActivityBranchBuilder Activity(Action<IActivityBuilder> activity) {
			var builder = new ActivityBuilder();
			activity(builder);

			Activities.Add(builder);
			return this;
		}

		public IActivityBranchBuilder If(Func<State, bool> decision) {
			Decision = decision;

			return this;
		}

		public IBranchActivity Build(IBuildContext context) {
			if (Activities.Count == 0)
				throw new InvalidOperationException("At least one activity must be defined in a branch");
			if (Strategy == null)
				throw new InvalidOperationException();

			var activities = Activities.Select(x => x.Build(context)).ToList();

			if (activities.Count == 1 &&
				activities[0] is MergeActivity)
				throw new InvalidOperationException();

			if (Factory) {
				if (StateFactory == null &&
					StateFactoryType == null)
					throw new InvalidOperationException();

				var stateFactory = StateFactory;

				if (stateFactory == null && context == null)
					throw new NotSupportedException();

				if (stateFactory == null)
					stateFactory = context.Resolve(StateFactoryType) as IStateFactory;

				return new BranchFactoryActivity(Name, Decision, activities, stateFactory);
			}


			return new BranchActivity(Name, Decision, Strategy, activities);
		}

		public ExecutionNode BuildNode() {
			if (Strategy == null)
				throw new InvalidOperationException();

			return new BuilderNode(Name, Decision != null, true, Strategy.IsParallel, Metadata) {
				InnerNodes = Activities.Select(x => x.BuildNode())
			};
		}

		public void AsFactory(IStateFactory stateFactory) {
			if (stateFactory == null)
				throw new ArgumentNullException(nameof(stateFactory));

			Factory = true;
			StateFactory = stateFactory;
		}

		public void AsFactory(Type factoryType) {
			if (factoryType == null)
				throw new ArgumentNullException(nameof(factoryType));

			if (!typeof(IStateFactory).GetTypeInfo().IsAssignableFrom(factoryType))
				throw new ArgumentException($"The type {factoryType} is not assignable from {typeof(IStateFactory)}");

			StateFactoryType = factoryType;
			Factory = true;
		}
	}
}