using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public class Workflow : IWorkflow, ICollection<IActivity> {
		private List<IActivity> activities;

		public Workflow() {
			activities = new List<IActivity>();
		}

		string IComponent.Name => WorkflowName;

		public virtual string WorkflowName {
			get { return GetType().Name; }
		}

		IBranchStrategy IBranch.Strategy => BranchStrategies.Sequential;

		public IEnumerable<IActivity> Activities => activities.AsReadOnly();

		public void Add(IActivity activity) {
			activities.Add(activity);
		}

		public void Add(string name, Func<State, CancellationToken, Task<State>> execution) {
			Add(name, state => true, execution);
		}

		public void Add(string name, Func<State, bool> decision, Func<State, CancellationToken, Task<State>> execution) {
			Add(new Activity(name, decision, execution));
		}

		public void Add(string name, Func<State, bool> decision, Func<State, Task<State>> execution) {
			Add(name, decision, (state, token) => execution(state));
		}

		public void Add(string name, Func<State, Task<State>> execution) {
			Add(name, state => true, execution);
		}

		public void Add(string name, Func<State, State> execution) {
			Add(name, state => true, execution);
		}

		public void Add(string name, Func<State, bool> decision, Func<State, State> execution) {
			Add(name, decision, (state, token) => Task.FromResult(execution(state)));
		}

		public void Add(string name, IMergeStrategy strategy) {
			Add(new MergeActivity(name, strategy));
		}

		public void Add(string name, IBranchStrategy strategy, params IActivity[] branched) {
			Add(name, null, strategy, branched);
		}

		public void Add(string name, Func<State, bool> decision, IBranchStrategy strategy, params IActivity[] branched) {
			Add(new BranchActivity(name, decision, strategy, branched));
		}

		public void Add(string name, IRepeatDecisor decisor) {
			Add(new RepeatActivity(name, decisor));
		}

		public static IWorkflowBuilder Build(Action<IWorkflowBuilder> workflow) {
			var builder = new WorkflowBuilder();
			workflow(builder);

			return builder;
		}

		#region ICollection<IActivity>

		IEnumerator IEnumerable.GetEnumerator() {
			return activities.GetEnumerator();
		}

		IEnumerator<IActivity> IEnumerable<IActivity>.GetEnumerator() {
			return activities.GetEnumerator();
		}

		void ICollection<IActivity>.Clear() {
			activities.Clear();
		}

		bool ICollection<IActivity>.Contains(IActivity item) {
			throw new NotSupportedException();
		}

		void ICollection<IActivity>.CopyTo(IActivity[] array, int arrayIndex) {
			activities.CopyTo(array, arrayIndex);
		}

		bool ICollection<IActivity>.Remove(IActivity item) {
			throw new NotSupportedException();
		}

		int ICollection<IActivity>.Count {
			get { return activities.Count; }
		}

		bool ICollection<IActivity>.IsReadOnly {
			get { return false; }
		}
		#endregion
	
	}
}