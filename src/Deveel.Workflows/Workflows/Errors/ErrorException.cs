using System;

namespace Deveel.Workflows.Errors
{
    public class ErrorException : FlowException, IError
    {
        public ErrorException(string errorName, string errorCode)
        {
            ErrorName = errorName;
            ErrorCode = errorCode;
        }

        public ErrorException(string errorName, string errorCode, string message) : base(message)
        {
            ErrorName = errorName;
            ErrorCode = errorCode;
        }

        public ErrorException(string errorName, string errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorName = errorName;
            ErrorCode = errorCode;
        }

        string IError.Name => ErrorName;

        string IError.Code => ErrorCode;

        public string ErrorName { get; }

        public string ErrorCode { get; }
    }
}
