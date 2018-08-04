using System;

namespace Deveel.Workflows.Errors
{
    public class ErrorException : FlowException, IError
    {
        public ErrorException(string errorName)
        {
            ErrorName = errorName;
        }

        public ErrorException(string errorName, string message) : base(message)
        {
            ErrorName = errorName;
        }

        public ErrorException(string errorName, string message, Exception innerException) : base(message, innerException)
        {
            ErrorName = errorName;
        }

        string IError.Name => ErrorName;

        public string ErrorName { get; }
    }
}
