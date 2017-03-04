using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Deveel.Workflows.Graph;

namespace Deveel.Workflows {
	/// <summary>
	/// Provides the state for activities during the execution 
	/// of a workflow.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Workflows consist of definitions of activities to be executed: by
	/// definition, the activities of a workflow are state-less and operate
	/// on the state provided as input from the preceding activities 
	/// </para>
	/// <para>
	/// Activities output states generated from the ones  provided as input,
	/// providing values and metadata specific for the activity: subsequent
	/// activities can decide the execution based on information provided from
	/// the input state.
	/// </para>
	/// <para>
	/// Executed states will be used to produce an execution graph to analyze
	/// the performances of the executed workflow.
	/// </para>
	/// <para>
	/// Components use the <see cref="ExecutionInfo"/> provided by a <see cref="State"/>
	/// to registers information about their own execution performances: the
	/// information will be used for analysis later on.
	/// </para>
	/// </remarks>
	public class State {
		/// <summary>
		/// Constructs an instance <see cref="State"/> that holds no value.
		/// </summary>
		/// <remarks>
		/// Constructing a <see cref="State"/> using this constructor
		/// breaks any execution graph and should be done only
		/// at the beginning of an execution plan: use <see cref="New(IComponent)"/>,
		/// <see cref="New(string)"/> or <see cref="NewBranch"/> to create
		/// a new instance of <see cref="State"/> that is chained to this one
		/// in the execution flow.
		/// </remarks>
		public State() 
			: this(null) {
		}

		/// <summary>
		/// Constructs an instance of <see cref="State"/> with a value specified.
		/// </summary>
		/// <param name="value">A value hold by this state</param>
		/// <remarks>
		/// Constructing a <see cref="State"/> using this constructor
		/// breaks any execution graph and should be done only
		/// at the beginning of an execution plan: use <see cref="New(IComponent)"/>,
		/// <see cref="New(string)"/> or <see cref="NewBranch"/> to create
		/// a new instance of <see cref="State"/> that is chained to this one
		/// in the execution flow.
		/// </remarks>
		public State(object value) 
			: this(null, null) {
			Value = value;
		}

		internal State(State previous, IComponent component) {
			Previous = previous;
			ExecutionInfo = new ExecutionInfo(component);

			if (previous != null)
				Value = previous.Value;

			Metadata = new Dictionary<string, object>();
		}

		/// <summary>
		/// Gets the reference to the state previously executed
		/// </summary>
		public State Previous { get; }

		/// <summary>
		/// Gets the information about the execution of the component
		/// that holds the state.
		/// </summary>
		public ExecutionInfo ExecutionInfo { get; }

		/// <summary>
		/// Gets a value hold by the state
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value can be considered as a transportation
		/// object from a given state to the next one: it is 
		/// possible to set a new value for this object using
		/// <see cref="SetValue"/>.
		/// </para>
		/// <para>
		/// A process uses the specification of a 
		/// </para>
		/// </remarks>
		/// <seealso cref="SetValue"/>
		public object Value { get; private set; }

		/// <summary>
		/// Gets a set of metadata defined by the state
		/// </summary>
		/// <remarks>
		/// <para>
		/// This collection is isolated within the state context:
		/// changes to the contents of the collection will not affect
		/// previous or future contexts.
		/// </para>
		/// <para>
		/// The resolution of a metadata i hierarchical: if a subsequent
		/// state defines an entry with the same key of an entry defined
		/// in a previous state, the latter one will be resolved, keeping
		/// the previous immutated, but not immediately visible.
		/// </para>
		/// </remarks>
		/// <seealso cref="GetMetadata"/>
		/// <seealso cref="GetMetadata{T}"/>
		/// <seealso cref="SetMetadata"/>
		public IDictionary<string, object> Metadata { get; }

		/// <summary>
		/// Gets a string that represents the execution path until
		/// this state
		/// </summary>
		/// <seealso cref="Path"/>
		public string PathString => String.Join("->", Path);

		/// <summary>
		/// Gets the list of the descriptive name of executed states 
		/// until this one.
		/// </summary>
		public string[] Path => BuildPath();

		/// <summary>
		/// Gets a value indicating if a new value was set
		/// through <see cref="SetValue"/>
		/// </summary>
		/// <seealso cref="SetValue"/>
		public bool HasNewValue { get; private set; }

		/// <summary>
		/// Gets a value indicating if this state represents the 
		/// entry point of an execution plan
		/// </summary>
		public bool IsEntry => Previous == null;

		/// <summary>
		/// Gets the state that follows this one in the execution graph 
		/// </summary>
		/// <remarks>
		/// A state is chained in an execution graph when it is created
		/// using the <see cref="New(IComponent)"/> and <see cref="New(string)"/>
		/// factory methods: this one becomes the previous of the new state, while
		/// the new state becomes the next of this one.
		/// </remarks>
		/// <seealso cref="New(IComponent)"/>
		/// <seealso cref="New(string)"/>
		public State Next { get; private set; }

		/// <summary>
		/// Gets the instance of the entry state in the execution graph
		/// </summary>
		/// <seealso cref="IsEntry"/>
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
			var name = state.IsEntry ? "[begin]" : GetComponentName(state.ExecutionInfo.Component);
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

		/// <summary>
		/// Sets a new value to hold for the current state.
		/// </summary>
		/// <param name="obj">A value to be hold by this state.</param>
		/// <returns>
		/// Returns an instance of this state with a new value set.
		/// </returns>
		/// <remarks>
		/// This method assigns a new value to <see cref="Value"/>
		/// and sets the flag <see cref="HasNewValue"/> to <c>true</c>.
		/// </remarks>
		public State SetValue(object obj) {
			Value = obj;
			HasNewValue = true;
			return this;
		}

		/// <summary>
		/// Constructs a new instance of <see cref="State"/>
		/// that is the next of this state in the execution graph
		/// </summary>
		/// <param name="component">The components against which the
		/// state is evaluated</param>
		/// <returns>
		/// Returns an instance of <see cref="State"/> that holds
		/// a reference to the given <paramref name="component"/>
		/// and is set as the next of this state in the execution graph
		/// </returns>
		public State New(IComponent component) {
			var state = new State(this, component);
			Next = state;
			return state;
		}

		/// <summary>
		/// Constructs a new instance of <see cref="State"/>
		/// that is the next of this state in the execution graph
		/// </summary>
		/// <param name="component">The name of a virtual component</param>
		/// <returns>
		/// Returns an instance of <see cref="State"/> that holds
		/// a reference to the given <paramref name="component"/>
		/// and is set as the next of this state in the execution graph
		/// </returns>
		/// <remarks>
		/// This overload is a convenience to provide a state to those
		/// executions that have no concrete implementation of
		/// <see cref="IComponent"/> (like dynamic activities).
		/// </remarks>
		public State New(string component) {
			return New(new VirtualComponent(component));
		}

		/// <summary>
		/// Constructs a new instance of <see cref="BranchState"/>
		/// that is the next of this state in the execution graph
		/// </summary>
		/// <param name="component">The components against which the
		/// state is evaluated</param>
		/// <returns>
		/// Returns an instance of <see cref="BranchState"/> that holds
		/// a reference to the given <paramref name="component"/>
		/// and is set as the next of this state in the execution graph
		/// </returns>
		/// <seealso cref="BranchState"/>
		public BranchState NewBranch(IComponent component) {
			var state = new BranchState(this, component);
			Next = state;
			return state;
		}

		/// <summary>
		/// Attempts to cast this state to a <see cref="BranchState"/>
		/// </summary>
		/// <returns>
		/// Returns an instance of this <see cref="State"/> casted
		/// as <see cref="BranchState"/>.
		/// </returns>
		/// <exception cref="InvalidCastException">If this state is not
		/// an instance of <see cref="BranchState"/></exception>
		public BranchState AsBranch() {
			if (!(this is BranchState))
				throw new InvalidCastException("This state is not a branch.");

			return (BranchState) this;
		}

		/// <summary>
		/// Gets a value indicating if this state is an instance of <see cref="BranchState"/>
		/// </summary>
		public bool IsBranch => this is BranchState;

		/// <summary>
		/// Gets a value from the metadata of the state in a
		/// hierarchical access.
		/// </summary>
		/// <param name="key">The key of the metadata to extract.</param>
		/// <returns>
		/// Returns a value corresponding to the provided key, if found in
		/// the hierarchy of the executed states until this, or <c>null</c>
		/// if not found.
		/// </returns>
		/// <remarks>
		/// This method walks back to the hierarchy of the executed states,
		/// including this, to attempt to find a metadata corresponding to
		/// the given key, and returns the last value set for the key.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="key"/>
		/// is <c>null</c> or empty</exception>
		public object GetMetadata(string key) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			var current = this;
			while (current != null) {
				object obj;
				if (current.Metadata.TryGetValue(key, out obj))
					return obj;

				current = current.Previous;
			}

			return null;
		}

		/// <summary>
		/// Gets a value from the metadata of the state in a
		/// hierarchical access, casting the result to the specified type.
		/// </summary>
		/// <param name="key">The key of the metadata to extract.</param>
		/// <returns>
		/// Returns a value corresponding to the provided key, if found in
		/// the hierarchy of the executed states until this, or the default
		/// value of <paramref name="{T}"/> if not found.
		/// </returns>
		/// <remarks>
		/// This method walks back to the hierarchy of the executed states,
		/// including this, to attempt to find a metadata corresponding to
		/// the given key, and returns the last value set for the key,
		/// converted to the specified <paramref name="{T}"/>.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="key"/>
		/// is <c>null</c> or empty</exception>
		/// <exception cref="InvalidCastException">If a value found for the given key
		/// is not convertible to <paramref name="{T}"/></exception>
		public T GetMetadata<T>(string key) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			var current = this;
			while (current != null) {
				if (current.Metadata.HasValue(key))
					return current.Metadata.GetValue<T>(key);

				current = current.Previous;
			}

			return default(T);
		}

		/// <summary>
		/// Checks if any metadata corresponding to the provided key
		/// is specified in the hierarchy of the executed states until
		/// this one.
		/// </summary>
		/// <param name="key">The key of the metadata to check.</param>
		/// <remarks>
		/// This method walks back to the hierarchy of the executed states,
		/// including this, to attempt to find a metadata corresponding to
		/// the given key.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="key"/>
		/// is <c>null</c> or empty</exception>
		public bool HasMetadata(string key) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			var current = this;
			while (current != null) {
				if (current.Metadata.HasValue(key))
					return true;

				current = current.Previous;
			}

			return false;
		}

		/// <summary>
		/// Sets the value of a metadata in the current state.
		/// </summary>
		/// <param name="key">The key of the value to set.</param>
		/// <param name="value">The value to set.</param>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="key"/>
		/// is <c>null</c> or empty</exception>
		/// <remarks>
		/// This methods sets a value only to the metadata of this state
		/// and has no impact on the values of metadata with the same key
		/// contained in previous states.
		/// </remarks>
		/// <seealso cref="Metadata"/>
		public void SetMetadata(string key, object value) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			Metadata[key] = value;
		}

		/// <summary>
		/// Attempts to return the value hold by this state as instance
		/// of the provided type, using an invariant culture for
		/// converting.
		/// </summary>
		/// <param name="type">The desired type of the returned object.</param>
		/// <returns>
		/// Returns an instance of <see cref="Value"/> converted to the
		/// desired destination <paramref name="type"/>, using an invariant
		/// culture for the conversion.
		/// </returns>
		/// <remarks>
		/// If <see cref="Value"/> is already an instance of the given
		///  <paramref name="type"/>, no conversion will be done.
		/// </remarks>
		/// <seealso cref="Value"/>
		/// <seealso cref="ValueAs(System.Type)"/>
		/// <seealso cref="ValueAs{T}()"/>
		/// <seealso cref="ValueAs{T}(IFormatProvider)"/>
		/// <exception cref="InvalidCastException">If <see cref="Value"/> is not
		/// convertible to the desired <paramref name="type"/></exception>
		/// <exception cref="ArgumentNullException">If the provided <param name="type"></param>
		/// is <c>null</c></exception>
		public object ValueAs(Type type) {
			return ValueAs(type, CultureInfo.InvariantCulture);
		}
		/// <summary>
		/// Attempts to return the value hold by this state as instance
		/// of the provided type, using the specified culture for
		/// converting.
		/// </summary>
		/// <param name="type">The desired type of the returned object.</param>
		/// <param name="formatProvider">The provider used to convert the source value
		/// to the desired target type.</param>
		/// <returns>
		/// Returns an instance of <see cref="Value"/> converted to the
		/// desired destination <paramref name="type"/>, using the provided
		/// format for the conversion.
		/// </returns>
		/// <remarks>
		/// If <see cref="Value"/> is already an instance of the given
		///  <paramref name="type"/>, no conversion will be done.
		/// </remarks>
		/// <seealso cref="Value"/>
		/// <seealso cref="ValueAs(Type)"/>
		/// <seealso cref="ValueAs{T}()"/>
		/// <seealso cref="ValueAs{T}(IFormatProvider)"/>
		/// <exception cref="InvalidCastException">If <see cref="Value"/> is not
		/// convertible to the desired <paramref name="type"/></exception>
		/// <exception cref="ArgumentNullException">If either the provided <paramref name="type"/>
		/// of <paramref name="formatProvider"/> is <c>null</c></exception>
		public object ValueAs(Type type, IFormatProvider formatProvider) {
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (formatProvider == null)
				throw new ArgumentNullException(nameof(formatProvider));

			var value = Value;

			if (!type.GetTypeInfo().IsInstanceOfType(value)) {
				var nullableType = Nullable.GetUnderlyingType(type);
				if (nullableType == null) {
					value = Convert.ChangeType(value, type, formatProvider);
				} else {
					value = Convert.ChangeType(value, nullableType, formatProvider);
				}
			}

			return value;
		}

		public T ValueAs<T>() {
			return ValueAs<T>(CultureInfo.InvariantCulture);
		}

		public T ValueAs<T>(IFormatProvider formatProvider) {
			return (T) ValueAs(typeof(T), formatProvider);
		}

		/// <summary>
		/// Generates a report on the execution graph until this state.
		/// </summary>
		/// <returns></returns>
		/// <seealso cref="ExecutionReport"/>
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