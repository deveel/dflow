using System;

namespace Deveel.Workflows
{
    public class FlowNodeException : FlowException
    {
        public FlowNodeException()
        {
        }

        public FlowNodeException(string message) : base(message)
        {
        }

        public FlowNodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
