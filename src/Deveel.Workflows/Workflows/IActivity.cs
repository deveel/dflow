using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows {
	public interface IActivity : IComponent {
		IEnumerable<KeyValuePair<string, object>> Metadata { get; }
			
		Task<State> ExecuteAsync(State state, CancellationToken cancellationToken);
	}
}