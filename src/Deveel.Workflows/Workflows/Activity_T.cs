using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public class Activity<TInput, TOutput> : Activity {

		public Activity()
			: base() {
		}

		public Activity(string name)
			: base(name, (Func<State, bool>) null) {
		}

		public Activity(string name, Func<State, CancellationToken, Task<State>> execution)
			: base(name, null, execution) {
		}

		public Activity(string name, Func<State, bool> decision, Func<State, CancellationToken, Task<State>> execution)
			: base(name, decision, execution) { 
		}

		protected override bool CanExecute(object obj) {
			return CanExecute((TInput) obj);
		}

		protected virtual bool CanExecute(TInput input) {
			return true;
		}

		protected override async Task<object> ExecuteValueAsync(object input, CancellationToken cancellationToken) {
			return (await ExecuteValueAsync((TInput) input, cancellationToken));
		}

		protected virtual Task<TOutput> ExecuteValueAsync(TInput input, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
}