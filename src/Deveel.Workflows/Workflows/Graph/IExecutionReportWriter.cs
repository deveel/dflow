using System;
using System.Threading;

namespace Deveel.Workflows.Graph {
	public interface IExecutionReportWriter {
		void WriteAsync(ExecutionReport report, CancellationToken cancellationToken);
	}
}