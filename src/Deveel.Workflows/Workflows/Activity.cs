using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public class Activity : IActivity {
		private List<ExecuteError> errorList;
		private Func<State, bool> decision;
		private Func<State, CancellationToken, Task<State>> execution;
		private string name;

		public Activity() 
			: this(null) {
		}

		public Activity(string name) 
			: this(name, null) {
		}

		public Activity(string name, Func<State, CancellationToken, Task<State>> execution) 
			: this(name, null, execution) {
		}

		public Activity(string name, Func<State, bool> decision, Func<State, CancellationToken, Task<State>> execution) {
			this.name = name;
			this.decision = decision;
			this.execution = execution;

			errorList = new List<ExecuteError>();
		}

		public virtual string ActivityName => FindName();

		string IComponent.Name => ActivityName;

		private string FindName() {
			if (!String.IsNullOrEmpty(name))
				return name;

			// TODO: find an attribute

			return GetType().Name;
		}

		protected void AddError(ExecuteError error) {
			errorList.Add(error);
		}

		bool IActivity.CanExecute(State state) {
			return CanExecute(state);
		}

		protected virtual bool CanExecute(State state) {
			return decision?.Invoke(state) ?? CanExecute(state.Value);
		}

		protected virtual bool CanExecute(object obj) {
			return true;
		}

		protected virtual async Task<State> ExecuteCurrentStateAsync(State state, CancellationToken cancellationToken) {
			if (execution != null) {
				return await execution(state, cancellationToken);
			}

			var result = await ExecuteValueAsync(state.Value, cancellationToken);
			state.SetValue(result);
			return state;
		}

		protected virtual State NextState(State previous) {
			return previous.GetNext(this);
		}

		public async Task<State> ExecuteAsync(State state, CancellationToken cancellationToken) {
			bool executed = false;

			var next = NextState(state);
			State result = next;

			try {
				next.StateInfo.Begin();

				if (CanExecute(state)) {
					executed = true;

					result = await ExecuteCurrentStateAsync(next, cancellationToken);

					if (errorList.Count > 0) {
						foreach (var error in errorList) {
							next.StateInfo.AddError(error);
						}
					}
				}
			} catch (Exception ex) {
				next.StateInfo.AddFatalError(new ExecuteError(ex));
			} finally {
				next.StateInfo.End(executed);
			}

			return result;
		}

		protected virtual Task<object> ExecuteValueAsync(object input, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
}