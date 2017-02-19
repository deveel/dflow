using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Deveel.Workflows {
	public sealed class StateInfo {
		private List<ExecuteError> errorList;
		private Stopwatch stopwatch;

		public StateInfo(IComponent component) {
			Component = component;
			stopwatch = new Stopwatch();
			errorList = new List<ExecuteError>();
		}

		public bool Failed { get; private set; }

		public IEnumerable<ExecuteError> Errors => errorList.AsReadOnly();

		public bool IsExecuting => stopwatch.IsRunning;

		public DateTimeOffset StartedAt { get; private set; }

		public DateTimeOffset? EndedAt { get; private set; }

		public TimeSpan Elapsed => stopwatch.Elapsed;

		public bool Executed { get; private set; }

		public IComponent Component { get; }

		internal void Begin() {
			StartedAt = DateTimeOffset.UtcNow;
			stopwatch.Start();
		}

		internal void End(bool executed = true) {
			EndedAt = DateTimeOffset.UtcNow;
			stopwatch.Stop();
			Executed = executed;
		}

		internal void AddFatalError(ExecuteError error) {
			Failed = true;
			errorList.Add(error);
		}

		internal void AddError(ExecuteError error) {
			errorList.Add(error);
		}
	}
}