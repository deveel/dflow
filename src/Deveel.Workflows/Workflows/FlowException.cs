using System;

namespace Deveel.Workflows
{
    public class FlowException : Exception
    {
        public FlowException()
        {
        }

        public FlowException(string message) : base(message)
        {
        }

        public FlowException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
