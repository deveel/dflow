﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Workflows.Graph;

namespace Deveel.Workflows {
	class ActivityBuilder : IActivityBuilder, INamedActivityBuilder, 
		IConditionalActivityBuilder, 
		IExecutionNodeBuilder, IActivityFactorableBuilder {
		private string Name { get; set; }

		private Dictionary<string, object> Metadata { get; set; }

		private Func<State, bool> Decision { get; set; }

		private Type ActivityType { get; set; }

		private ActivityBranchBuilder ActivityBranchBuilder { get; set; }

		private IActivity ProxyActivity { get; set; }

		private Func<State, CancellationToken, Task<State>> Execution { get; set; }

		private bool Factory { get; set; }

		private IStateFactory StateFactory { get; set; }

		private bool OptionSet { get; set; }

		private void AssertOptionNotSet() {
			if (OptionSet)
				throw new InvalidOperationException("An activity build option has already been set");
		}

		public INamedActivityBuilder Named(string name) {
			Name = name;
			return this;
		}

		public INamedActivityBuilder With(string key, object metadata) {
			if (Metadata == null)
				Metadata = new Dictionary<string, object>();

			Metadata[key] = metadata;
			return this;
		}

		public IActivity Build(IBuildContext context) {
			if (!OptionSet)
				throw new ActivityBuildException("No valid option was set for the build of an activity");

			try {
				IActivity activity;

				if (ActivityType != null) {
					if (context == null)
						throw new ActivityResolveException(ActivityType, "The builder references a type that cannot be resolved outside a context");

					activity = context.ResolveActivity(ActivityType);
				} else if (ProxyActivity != null) {
					activity = ProxyActivity;
				} else if (ActivityBranchBuilder != null) {
					activity = ActivityBranchBuilder.Build(context);
				} else {
					activity = new Activity(Name, Decision, Execution);

					if (Metadata != null) {
						((Activity)activity).SetMetadata(Metadata);
					}
				}

				if (Factory) {
					if (StateFactory == null)
						throw new ActivityBuildException("The activity is indicated to be a factory but no state factory was specified.");

					activity = new ActivityFactoryActivity(Name, Decision, activity, StateFactory);
				}

				return activity;
			} catch (ActivityBuildException) {
				throw;
			} catch (Exception ex) {
				throw new ActivityBuildException("Could not build the activity because of an error: see inner exception for details.", ex);
			}
		}

		public void AsFactory(IStateFactory stateFactory) {
			if (stateFactory == null)
				throw new ArgumentNullException(nameof(stateFactory));

			Factory = true;
			StateFactory = stateFactory;
		}

		public ExecutionNode BuildNode() {
			if (!OptionSet)
				throw new InvalidOperationException();

			ExecutionNode node;

			if (ActivityType != null) {
				throw new NotImplementedException("Projection in the graph of a reference type not supported");
			} else if (ProxyActivity != null) {
				node = new ComponentNode(ProxyActivity);
			} else if (ActivityBranchBuilder != null) {
				node = ActivityBranchBuilder.BuildNode();
			} else {
				node = new BuilderNode(Name, Decision != null, false, false, Metadata);
			}

			return node;
		}

		public IConditionalActivityBuilder If(Func<State, bool> decision) {
			Decision = decision;
			return this;
		}

		public void Branch(Action<IActivityBranchBuilder> branch) {
			AssertOptionNotSet();

			var builder = new ActivityBranchBuilder();
			branch(builder);

			ActivityBranchBuilder = builder;
			OptionSet = true;
		}

		public IActivityFactorableBuilder OfType(Type type) {
			AssertOptionNotSet();

			ActivityType = type;
			OptionSet = true;
			return this;
		}

		public IActivityFactorableBuilder Proxy(IActivity activity) {
			AssertOptionNotSet();

			ProxyActivity = activity;
			OptionSet = true;
			return this;
		}

		public IActivityFactorableBuilder Execute(Func<State, CancellationToken, Task<State>> execution) {
			AssertOptionNotSet();

			Execution = execution;
			OptionSet = true;
			return this;
		}
	}
}