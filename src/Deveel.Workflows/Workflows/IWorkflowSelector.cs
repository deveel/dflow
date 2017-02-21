using System;

namespace Deveel.Workflows {
	public interface IWorkflowSelector {
		string SelectWorkflow(State state);
	}
}