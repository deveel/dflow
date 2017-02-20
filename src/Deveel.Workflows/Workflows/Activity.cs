using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public class Activity : IActivity {
		private List<ExecuteError> errorList;
		private Func<State, bool> decision;
		private Func<State, CancellationToken, Task<State>> execution;
		private string name;
		private Dictionary<string, object> meta;

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

			meta = new Dictionary<string, object>();
			errorList = new List<ExecuteError>();

			EnsureMetadata();
		}

		public virtual string ActivityName => FindName();

		string IComponent.Name => ActivityName;

		public IEnumerable<KeyValuePair<string, object>> Metadata => meta.AsEnumerable();

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

		protected virtual void GetMetadata(IDictionary<string, object> metadata) {
			
		}

		protected void AddError(ExecuteError error) {
			errorList.Add(error);
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
			return previous.New(this);
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