using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Graph {
	public interface IReportWriter {
		Task WriteAsync(ExecutionReport report, CancellationToken cancellationToken);
	}
}