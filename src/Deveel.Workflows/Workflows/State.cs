using System;
using System.Collections.Generic;
using System.Linq;

using Deveel.Workflows.Graph;

namespace Deveel.Workflows {
	public class State {
		public State() 
			: this(null) {
		}

		public State(object obj) 
			: this(null, null) {
			Value = obj;
		}

		internal State(State previous, IComponent component) {
			Previous = previous;
			StateInfo = new StateInfo(component);

			if (previous != null)
				Value = previous.Value;

			Metadata = new Dictionary<string, object>();
		}

		public State Previous { get; }

		public StateInfo StateInfo { get; }

		public object Value { get; private set; }

		public IDictionary<string, object> Metadata { get; }

		public string PathString => String.Join("->", Path);

		public string[] Path => BuildPath();

		public bool HasNewValue { get; private set; }

		public bool IsEntry => Previous == null;

		public State Next { get; private set; }

		public State Entry {
			get {
				if (IsEntry)
					return this;

				var current = this;
				while (current != null) {
					if (current.IsEntry)
						return current;

					current = current.Previous;
				}

				throw new InvalidOperationException("Could not define any entry state");
			}
		}

		private static string GetComponentName(IComponent component) {
			var name = component.Name;
			if (component is IBranch) {
				var branch = (IBranch)component;
				var sep = branch.Strategy.IsParallel ? "|" : ",";
				var children = String.Join(sep, branch.Activities.Select(GetComponentName));
				name = String.Format("{0}[{1}]", name, children);
			}

			return name;
		}

		private static string GetStateName(State state) {
			var name = state.IsEntry ? "[begin]" : GetComponentName(state.StateInfo.Component);
			if (String.IsNullOrEmpty(name))
				name = "<unnamed activity>";

			return name;
		}

		private string[] BuildPath() {
			var names = new List<string>();

			var current = this;
			while (current != null) {
				var name = GetStateName(current);
				names.Add(name);

				current = current.Previous;
			}

			names.Reverse();
			return names.ToArray();
		}

		public State SetValue(object obj) {
			Value = obj;
			HasNewValue = true;
			return this;
		}

		public State New(IComponent component) {
			var state = new State(this, component);
			Next = state;
			return state;
		}

		public State New(string component) {
			return New(new VirtualComponent(component));
		}

		public State NewBranch(IComponent component) {
			var state = new BranchState(this, component);
			Next = state;
			return state;
		}

		public BranchState AsBranch() {
			if (!(this is BranchState))
				throw new InvalidCastException();

			return (BranchState) this;
		}

		public bool IsBranch => this is BranchState;

		public object GetMetadata(string key) {
			var current = this;
			while (current != null) {
				object obj;
				if (current.Metadata.TryGetValue(key, out obj))
					return obj;

				current = current.Previous;
			}

			return null;
		}

		public T GetMetadata<T>(string key) {
			var current = this;
			while (current != null) {
				if (current.Metadata.HasValue(key))
					return current.Metadata.GetValue<T>(key);

				current = current.Previous;
			}

			return default(T);
		}

		public bool HasMetadata(string key) {
			var current = this;
			while (current != null) {
				if (current.Metadata.HasValue(key))
					return true;

				current = current.Previous;
			}

			return false;
		}

		public void SetMetadata(string key, object value) {
			Metadata[key] = value;
		}

		public ExecutionReport GetReport() {
			return ExecutionReport.Build(this);
		}

		#region VirtualComponent

		class VirtualComponent : IComponent {
			public VirtualComponent(string name) {
				Name = name;
			}

			public string Name { get; }
		}

		#endregion
	}
}