using System;

namespace Deveel.Workflows.BusinessRules
{
    public class RuleException : FlowException
    {
        public RuleException()
        {
        }

        public RuleException(string message) : base(message)
        {
        }

        public RuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
