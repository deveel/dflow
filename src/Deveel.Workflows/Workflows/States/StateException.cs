using System;

namespace Deveel.Workflows.States
{
    public class StateException : FlowException
    {
        public StateException()
        {
        }

        public StateException(string message) : base(message)
        {
        }

        public StateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
