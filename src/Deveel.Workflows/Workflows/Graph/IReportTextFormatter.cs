using System;
using System.IO;
using System.Threading.Tasks;

namespace Deveel.Workflows.Graph {
	public interface IReportTextFormatter {
		Task WriteAsync(TextWriter writer, ExecutionReport report);
	}
}