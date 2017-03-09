using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	/// <summary>
	/// Represents a default implementation of <see cref="IActivity"/>
	/// contract to provide a default set of features for 
	/// activities in a workflow
	/// </summary>
	public class Activity : IActivity {
		private List<ExecuteError> errorList;
		private Func<State, bool> decision;
		private Func<State, CancellationToken, Task<State>> execution;
		private string name;
		private Dictionary<string, object> meta;
		private bool? hasDecision;

		/// <summary>
		/// Constructs an instance of <see cref="Activity"/>
		/// that is unnamed and has no decision or execution
		/// </summary>
		public Activity() 
			: this(null) {
		}

		/// <summary>
		/// Constructs a named instance of <see cref="Activity"/>
		/// that has no decision or execution
		/// </summary>
		/// <param name="name">The name of the activity in the graph</param>
		public Activity(string name) 
			: this(name, (Func<State, CancellationToken, Task<State>>)null) {
		}

		/// <summary>
		/// Constructs a named instance of <see cref="Activity"/>
		/// that has also an execution body but no decision
		/// </summary>
		/// <param name="name">The name of the activity in the graph</param>
		/// <param name="execution">The delegated function that represents
		/// the body of the activity to be executed</param>
		public Activity(string name, Func<State, CancellationToken, Task<State>> execution) 
			: this(name, null, execution) {
		}

		/// <summary>
		/// Constructs a named instance of <see cref="Activity"/>
		/// that has a decision but no execution body.
		/// </summary>
		/// <param name="name">The name of the activity in the graph</param>
		/// <param name="decision">The delegated function for the decision
		/// on the execution of the activity</param>
		public Activity(string name, Func<State, bool> decision) 
			: this(name, decision, null) {
		}

		/// <summary>
		/// Constructs a named instance of <see cref="Activity"/> that
		/// includes the decision and the execution bodies
		/// </summary>
		/// <param name="name">The name of the activity in the graph</param>
		/// <param name="decision">The delegated function for the decision
		/// on the execution of the activity</param>
		/// <param name="execution">The delegated function that is
		/// executed as body of the activity</param>
		public Activity(string name, Func<State, bool> decision, Func<State, CancellationToken, Task<State>> execution) {
			this.name = name;
			this.decision = decision;
			this.execution = execution;

			meta = new Dictionary<string, object>();
			errorList = new List<ExecuteError>();

			EnsureMetadata();
		}

		/// <summary>
		/// Gets the name of activity
		/// </summary>
		/// <remarks>
		/// <para>
		/// The name of an activity can be specified using one
		/// of the following methods:
		/// <list type="bullet">
		/// <listitem>Specifying the attribute <see cref="ActivityNameAttribute"/> to the
		/// inherited instance of the activity</listitem>
		/// <listitem>Overriding this property to explicitly return a new name</listitem>
		/// <listitem>Providing a neme in one of the constructors of <see cref="Activity"/></listitem>
		/// </list>
		/// </para>
		/// <para>
		/// Activity names are merely descriptive and they are not 
		/// required to be unique withing a workflow or branch
		/// </para>
		/// <para>
		/// The default name of an activity inheriting <see cref="Activity"/>
		/// is the type name of the inheriting class.
		/// </para>
		/// </remarks>
		public virtual string ActivityName => FindName();

		string IComponent.Name => ActivityName;

		/// <inheritdoc cref="IActivity"/>
		public IEnumerable<KeyValuePair<string, object>> Metadata => meta.AsEnumerable();

		/// <inheritdoc cref="IActivity"/>
		public virtual bool HasDecision => FindHasDecision();


		private bool FindHasDecision() {
			if (hasDecision == null) {
				if (decision != null) {
					hasDecision = true;
				} else if (this.HasMetadata(KnownActivityMetadataKeys.Decides)) {
					hasDecision = this.GetMetadata<bool>(KnownActivityMetadataKeys.Decides);
				} else {
					hasDecision = OverridesCanExecute();
				}
			}

			return hasDecision.Value;
		}

		private bool OverridesCanExecute() {
			var activityType = typeof(Activity).GetTypeInfo();
			var thisType = GetType();

			var methods = activityType.FindMembers(MemberTypes.Method,
				BindingFlags.NonPublic | BindingFlags.Instance,
				(info, criteria) => info is MethodInfo && info.Name == nameof(CanExecute),
				null);

			return methods.Any(x => x.DeclaringType == thisType && thisType != activityType.AsType());
		}

		private void EnsureMetadata() {
			FindMetadataFromAttributes();
			GetMetadata(meta);
		}

		private void FindMetadataFromAttributes() {
			var typeInfo = GetType().GetTypeInfo();
			var attributes = typeInfo.GetCustomAttributes<ActivityMetadataAttribute>();
			foreach (var attribute in attributes) {
				meta[attribute.Key] = attribute.Value;
			}
		}

		private string FindName() {
			var result = name;

			if (!String.IsNullOrEmpty(result))
				return result;

			result = this.GetMetadata<string>(KnownActivityMetadataKeys.Name);

			if (!String.IsNullOrEmpty(result))
				return result;

			return GetType().Name;
		}

		/// <summary>
		/// When overridden from an inheriting class, populates the
		/// provided dictionary of metadata
		/// </summary>
		/// <param name="metadata"></param>
		protected virtual void GetMetadata(IDictionary<string, object> metadata) {
			
		}

		internal void SetMetadata(IEnumerable<KeyValuePair<string, object>> metadata) {
			meta = metadata.ToDictionary(x => x.Key, y => y.Value);
		}

		/// <summary>
		/// Adds an execution error in the temporary context
		/// of the activity
		/// </summary>
		/// <param name="error">The error to be added</param>
		/// <remarks>
		/// This method is a convenience when executing the body
		/// of <see cref="ExecuteAsync"/>, to be collected later
		/// </remarks>
		/// <seealso cref="ExecuteAsync"/>
		protected void AddError(ExecuteError error) {
			errorList.Add(error);
		}

		/// <summary>
		/// Determines if the activity should be executed, given the input
		/// state coming from the previous activity.
		/// </summary>
		/// <param name="state">The input state used to decide if to
		/// execute the body of the activity</param>
		/// <returns>
		/// Returns <c>true</c> if the activity can be executed
		/// given the input state, coming from the preceding activity,
		/// otherwise <c>false</c>.
		/// </returns>
		/// <remarks>
		/// The default implementation delegates the decision to a
		/// function passed at construction, if any, or to <see cref="CanExecute(object)"/>
		/// passing the <see cref="State.Value"/>.
		/// </remarks>
		/// <exception cref="ArgumentNullException">If the provided <paramref name="state"/>
		/// is <c>null</c>.</exception>
		/// <seealso cref="HasDecision"/>
		protected virtual bool CanExecute(State state) {
			if (state == null)
				throw new ArgumentNullException(nameof(state));

			return decision?.Invoke(state) ?? CanExecute(state.Value);
		}

		/// <summary>
		/// Determines if the activity should be executed, given the input
		/// state value coming from the previous activity.
		/// </summary>
		/// <param name="obj">The value of the previous state</param>
		/// <returns>
		/// Returns <c>true</c> if the activity can be executed
		/// given the input value, coming from the preceding activity,
		/// otherwise <c>false</c>.
		/// </returns>
		/// <seealso cref="State.Value"/>
		protected virtual bool CanExecute(object obj) {
			return true;
		}

		/// <summary>
		/// Executes the routines of the current activity against
		/// the current state.
		/// </summary>
		/// <param name="state">The state current to this activity.</param>
		/// <param name="cancellationToken">A token used to notify the
		/// cancellation of the execution</param>
		/// <returns>
		/// Returns the current state after the execution of this activity
		/// </returns>
		/// <remarks>
		/// <para>
		/// If a body for execution was specified at construction, that function is
		/// used, otherwise the method falls back to <see cref="ExecuteValueAsync"/>,
		/// passing the <see cref="State.Value"/> wrapped in the current state. 
		/// </para>
		/// </remarks>
		/// <seealso cref="State.Value"/>
		/// <seealso cref="ExecuteAsync"/>
		protected virtual async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			if (execution != null) {
				return await execution(state, cancellationToken);
			}

			var result = await ExecuteValueAsync(state.Value, cancellationToken);
			state.SetValue(result);
			return state;
		}

		/// <summary>
		/// Creates a new state that is next to the provided one
		/// </summary>
		/// <param name="previous">The state coming from the execution
		/// of the previous activity.</param>
		/// <returns>
		/// Returns a new instance of <see cref="State"/> that
		/// is the next state of the given input state.
		/// </returns>
		/// <seealso cref="State.New(IComponent)"/>
		protected virtual State NextState(State previous) {
			return previous.New(this);
		}

		/// <summary>
		/// Executes the routines of an activity against the
		/// input state coming from the previous execution in the flow
		/// </summary>
		/// <param name="state">The state provided from the execution of 
		/// the previous activity in the flow</param>
		/// <param name="cancellationToken">A token used to signal the cancellation
		/// of the execution</param>
		/// <returns>
		/// Returns a new instance of <see cref="State"/> that encapsulates the
		/// result of this activity.
		/// </returns>
		/// <exception cref="ActivityExecutionException">If a fatal error occurred during
		/// the execution of the activity.</exception>
		/// <seealso cref="IActivity.ExecuteAsync"/>
		public async Task<State> ExecuteAsync(State state, CancellationToken cancellationToken) {
			bool executed = false;

			var next = NextState(state);
			State result = next;

			try {
				next.ExecutionInfo.Begin();

				if (CanExecute(state)) {
					executed = true;

					result = await ExecuteCurrentStateAsync(next, cancellationToken);

					if (errorList.Count > 0) {
						foreach (var error in errorList) {
							next.ExecutionInfo.AddError(error);
						}
					}
				}
			}catch(ActivityExecutionException) {
				throw;
			} catch (Exception ex) {
				next.ExecutionInfo.AddFatalError(new ExecuteError(ex));
			} finally {
				next.ExecutionInfo.End(executed);
			}

			return result;
		}

		/// <summary>
		/// Executes the routines of the activity against the
		/// value of the current state.
		/// </summary>
		/// <param name="input">The value of the current state used to execute
		/// the activity</param>
		/// <param name="cancellationToken">A token used to signal the cancellation
		/// of the execution</param>
		/// <returns>
		/// Returns the result of the execution of the activity.
		/// </returns>
		protected virtual Task<object> ExecuteValueAsync(object input, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
}