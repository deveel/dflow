using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Graph {
	public class GraphTextWriter : IGraphWriter {
		public GraphTextWriter(TextWriter textWriter, IGraphTextFormatter formatter) {
			if (textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));

			TextWriter = textWriter;
			TextFormatter = formatter;
		}

		public TextWriter TextWriter { get; }

		public IGraphTextFormatter TextFormatter { get; }

		public Task WriteAsync(ExecutionGraph graph, CancellationToken cancellationToken) {
			return TextFormatter.FormatGraphAsync(graph, TextWriter, cancellationToken);
		}
	}
}