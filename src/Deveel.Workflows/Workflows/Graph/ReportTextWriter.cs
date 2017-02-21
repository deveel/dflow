using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Graph {
	public class ReportTextWriter : IReportWriter {
		private TextWriter writer;

		public ReportTextWriter(TextWriter writer, IReportTextFormatter formatter) {
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			this.writer = writer;
			Formatter = formatter;
		}

		public IReportTextFormatter Formatter { get; }

		public Task WriteAsync(ExecutionReport report, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();
			return Formatter.WriteAsync(writer, report);
		}
	}
}