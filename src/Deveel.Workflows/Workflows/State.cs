using System;
using System.Collections.Generic;
using System.Linq;

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

		private static string GetStateName(State state) {
			var name = state.IsEntry ? "[begin]" : state.StateInfo.Component.Name;
			if (String.IsNullOrEmpty(name))
				name = "<unnamed activity>";

			return name;
		}

		private string[] BuildPath() {
			var names = new List<string>();

			var current = this;
			while (current != null) {
				var name = GetStateName(current);

				if (current is ParallelState) {
					name = String.Format("{0}[{1}]", name, 
						String.Join(",", current.AsParallel().States.Select(GetStateName)));
				}

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

		public State GetNext(IComponent component) {
			var state = new State(this, component);
			Next = state;
			return state;
		}

		public State GetNext(string component) {
			return GetNext(new VirtualComponent(component));
		}

		public State GetNextParallel(IComponent component) {
			var state = new ParallelState(this, component);
			Next = state;
			return state;
		}

		public ParallelState AsParallel() {
			if (!(this is ParallelState))
				throw new InvalidCastException();

			return (ParallelState) this;
		}

		public bool IsParallel => this is ParallelState;

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