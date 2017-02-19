using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface IActivity : IComponent {
		bool CanExecute(State state);

		Task<State> ExecuteAsync(State state, CancellationToken cancellationToken);
	}
}