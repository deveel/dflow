using System;

namespace Deveel.Workflows.Expressions
{
    public class FlowExpressionException : FlowException
    {
        public FlowExpressionException()
        {
        }

        public FlowExpressionException(string message) : base(message)
        {
        }

        public FlowExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
