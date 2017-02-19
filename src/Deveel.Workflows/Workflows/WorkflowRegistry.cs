using System;
using System.Collections.Generic;
using System.Threading;

namespace Deveel.Workflows {
	public abstract class WorkflowRegistry : IDisposable {
		private bool initialied;
		private bool disposed;
		private Dictionary<string, IWorkflowBuilder> builders;
		private Dictionary<string, Workflow> workflows;

		protected WorkflowRegistry(IBuildContext context) {
			Context = context;

			builders = new Dictionary<string, IWorkflowBuilder>();
			workflows = new Dictionary<string, Workflow>();

			EnsureInitialized();
		}

		~WorkflowRegistry() {
			Dispose(false);
		}

		protected IBuildContext Context { get; }

		protected abstract void Initialize();

		private void EnsureInitialized() {
			if (!initialied) {
				Initialize();
				initialied = true;
			}
		}

		protected void ThrowIfDisposed() {
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		protected void Register(string name, Action<IWorkflowBuilder> workflow) {
			ThrowIfDisposed();

			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if (workflows.ContainsKey(name))
				throw new ArgumentException($"A workflow named '{name}' is already registered.");

			var builder = new WorkflowBuilder();
			workflow(builder);

			builders[name] = builder;
		}

		protected void Register(string name, Workflow workflow) {
			ThrowIfDisposed();

			if (workflow == null)
				throw new ArgumentNullException(nameof(workflow));
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if (builders.ContainsKey(name))
				throw new ArgumentException($"A workflow named '{name}' is already registered.");

			workflows[name] = workflow;
		}

		public IWorkflow GetWorkflow(string name) {
			ThrowIfDisposed();

			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			EnsureInitialized();

			Workflow workflow;
			if (workflows.TryGetValue(name, out workflow))
				return workflow;

			IWorkflowBuilder builder;
			if (!builders.TryGetValue(name, out builder))
				return null;

			return builder.Build(Context);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing) {
					if (workflows != null)
						workflows.Clear();
					if (builders != null)
						builders.Clear();
				}

				workflows = null;
				builders = null;
				disposed = true;
			}
		}
	}
}