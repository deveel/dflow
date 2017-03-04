using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Deveel.Workflows {
	/// <summary>
	/// Handles the information about the execution of a
	/// component of a workflow.
	/// </summary>
	public sealed class ExecutionInfo {
		private List<ExecuteError> errorList;
		private Stopwatch stopwatch;

		internal ExecutionInfo(IComponent component) {
			Component = component;
			stopwatch = new Stopwatch();
			errorList = new List<ExecuteError>();
		}

		/// <summary>
		/// Gets a value indicating if the execution of the component
		/// failed and it was not possible to be completed.
		/// </summary>
		/// <remarks>
		/// This value is set to <c>true</c> when the component
		/// invokes <see cref="AddFatalError"/> method 
		/// </remarks>
		/// <seealso cref="AddFatalError"/>
		public bool Failed { get; private set; }

		/// <summary>
		/// Gets a list of all the errors that occurred during the execution
		/// of the component.
		/// </summary>
		public IEnumerable<ExecuteError> Errors => errorList.AsReadOnly();

		/// <summary>
		/// Gets a value indicating if the component is still executing
		/// </summary>
		/// <remarks>
		/// This information is particularly useful in the context of parallel
		/// execution of activities.
		/// </remarks>
		public bool IsExecuting => stopwatch.IsRunning;

		/// <summary>
		/// Gets the time-stamp of the start of the execution
		/// </summary>
		/// <remarks>
		/// This value is set when invoking <see cref="Begin"/> method
		/// </remarks>
		/// <seealso cref="Begin"/>
		public DateTimeOffset StartedAt { get; private set; }

		/// <summary>
		/// Gets the time-stamp of the end of the execution
		/// </summary>
		/// <remarks>
		/// The value of this property is set when <see cref="End"/>
		/// is invoked.
		/// </remarks>
		/// <seealso cref="End"/>
		public DateTimeOffset? EndedAt { get; private set; }

		/// <summary>
		/// Gets the elapsed time since the beginning of the execution
		/// </summary>
		public TimeSpan Elapsed => stopwatch.Elapsed;

		/// <summary>
		/// Gets a value indicating if the component was executed or if
		/// any decision logic prohibited the execution
		/// </summary>
		public bool Executed { get; private set; }

		/// <summary>
		/// Gets the instance of the component being executed.
		/// </summary>
		public IComponent Component { get; }

		/// <summary>
		/// Begin the registration of the information of the execution
		/// </summary>
		/// <remarks>
		/// The execution must always have a start point, even if the
		/// concrete execution is not performed.
		/// </remarks>
		/// <seealso cref="StartedAt"/>
		public void Begin() {
			StartedAt = DateTimeOffset.UtcNow;
			stopwatch.Start();
		}

		/// <summary>
		/// Indicates the end of the execution of a component
		/// </summary>
		/// <param name="executed">Specifies whether the real execution of
		/// the component took place or if a decision point prevented it.</param>
		/// <seealso cref="Begin"/>
		/// <seealso cref="Executed"/>
		public void End(bool executed = true) {
			EndedAt = DateTimeOffset.UtcNow;
			stopwatch.Stop();
			Executed = executed;
		}

		/// <summary>
		/// Registers an error that caused the execution to fail.
		/// </summary>
		/// <param name="error">The error that caused the failure.</param>
		/// <seealso cref="Failed"/>
		public void AddFatalError(ExecuteError error) {
			Failed = true;
			errorList.Add(error);
		}

		/// <summary>
		/// Registers a non-fatal error that occurs during the execution
		/// </summary>
		/// <param name="error">The error occurred</param>
		public void AddError(ExecuteError error) {
			errorList.Add(error);
		}
	}
}