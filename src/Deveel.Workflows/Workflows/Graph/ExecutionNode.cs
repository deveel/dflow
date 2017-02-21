using System;
using System.Collections.Generic;

namespace Deveel.Workflows.Graph
{
    abstract class ExecutionNode : IExecutionNode
    {
	    protected ExecutionNode() {
		    InnerNodes = new List<ExecutionNode>();
	    }

	    public abstract string Name { get; }

	    public virtual bool HasDecision => false;

	    public IExecutionNode Previous { get; private set; }

	    public IExecutionNode Next { get; private set; }

	    public abstract IDictionary<string, object> Metadata { get; }

	    public virtual IEnumerable<IExecutionNode> Nodes => InnerNodes;

		public IEnumerable<ExecutionNode> InnerNodes { get; set; }

		public void SetNext(ExecutionNode node) {
			Next = node;
			node.Previous = this;
		}

		public void SetPrevious(ExecutionNode node) {
			Previous = node;
			node.Next = this;
		}
	}
}
