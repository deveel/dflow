using System;
using System.Collections.Generic;
using System.Threading;

namespace Deveel.Workflows {
	public class WorkflowRegistry : IDisposable {
		private bool initialied;
		private bool disposed;
		private Dictionary<string, IWorkflowBuilder> builders;
		private Dictionary<string, Workflow> workflows;

		public WorkflowRegistry(IBuildContext context) 
			: this(context, null) {
		}

		public WorkflowRegistry(IWorkflowSelector selector) 
			: this(null, selector) {
		}

		public WorkflowRegistry(IBuildContext context, IWorkflowSelector selector) {
			Context = context;
			Selector = selector;

			builders = new Dictionary<string, IWorkflowBuilder>();
			workflows = new Dictionary<string, Workflow>();

			EnsureInitialized();
		}

		public WorkflowRegistry()
			: this(null, null) {
		}

		~WorkflowRegistry() {
			Dispose(false);
		}

		protected IBuildContext Context { get; }

		protected  IWorkflowSelector Selector { get; }

		protected virtual void Initialize() {
			
		}

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

		public void Register(string name, Action<IWorkflowBuilder> workflow) {
			ThrowIfDisposed();

			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			if (workflows.ContainsKey(name))
				throw new ArgumentException($"A workflow named '{name}' is already registered.");

			var builder = new WorkflowBuilder();
			workflow(builder);

			builders[name] = builder;
		}

		public void Register(string name, Workflow workflow) {
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
			return GetWorkflow(Context, name);
		}

		public IWorkflow GetWorkflow(IBuildContext context, string name) {
			ThrowIfDisposed();

			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			EnsureInitialized();

			Workflow workflow;
			if (workflows.TryGetValue(name, out workflow))
				return workflow;

			IWorkflowBuilder builder;
			if (!builders.TryGetValue(name, out builder))
				return null;

			return builder.Build(context);
		}

		public IWorkflow GetWorkflow(State state) {
			ThrowIfDisposed();

			if (state == null)
				throw new ArgumentNullException(nameof(state));

			if (Selector == null)
				throw new NotSupportedException();

			var name = Selector.SelectWorkflow(state);
			if (String.IsNullOrEmpty(name))
				return null;

			return GetWorkflow(name);
		}

		public IWorkflow GetWorkflow(object value) {
			return GetWorkflow(new State(value));
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