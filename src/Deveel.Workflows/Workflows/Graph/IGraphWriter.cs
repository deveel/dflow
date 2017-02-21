using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Graph {
	public interface IGraphWriter {
		Task WriteAsync(ExecutionGraph graph, CancellationToken cancellationToken);
	}
}