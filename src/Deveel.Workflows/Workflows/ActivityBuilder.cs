using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	class ActivityBuilder : IActivityBuilder, INamedActivityBuilder, IConditionalActivityBuilder {
		private string Name { get; set; }

		private Func<State, bool> Decision { get; set; }

		private Type ActivityType { get; set; }

		private IBranchBuilder BranchBuilder { get; set; }

		private IActivity ProxyActivity { get; set; }

		private Func<State, CancellationToken, Task<State>> Execution { get; set; }

		private bool OptionSet { get; set; }

		private void AssertOptionNotSet() {
			if (OptionSet)
				throw new InvalidOperationException("An activity build option has already been set");
		}

		public INamedActivityBuilder Named(string name) {
			Name = name;
			return this;
		}

		public IActivity Build(IBuildContext context) {
			if (!OptionSet)
				throw new InvalidOperationException();

			IActivity activity;

			if (ActivityType != null) {
				if (context == null)
					throw new NotSupportedException("The builder references a type that cannot be resolved outside a context");

				activity = context.ResolveActivity(ActivityType);
			} else if (ProxyActivity != null) {
				activity = ProxyActivity;
			} else if (BranchBuilder != null) {
				activity = BranchBuilder.Build(context);
			} else {
				activity = new Activity(Name, Decision, Execution);
			}

			return activity;
		}

		public IConditionalActivityBuilder If(Func<State, bool> decision) {
			Decision = decision;
			return this;
		}

		public void Branch(Action<IBranchBuilder> branch) {
			AssertOptionNotSet();

			var builder = new BranchBuilder();
			branch(builder);

			BranchBuilder = builder;
			OptionSet = true;
		}

		public void OfType(Type type) {
			AssertOptionNotSet();

			ActivityType = type;
			OptionSet = true;
		}

		public void Proxy(IActivity activity) {
			AssertOptionNotSet();

			ProxyActivity = activity;
			OptionSet = true;
		}

		public void Execute(Func<State, CancellationToken, Task<State>> execution) {
			AssertOptionNotSet();

			Execution = execution;
			OptionSet = true;
		}
	}
}