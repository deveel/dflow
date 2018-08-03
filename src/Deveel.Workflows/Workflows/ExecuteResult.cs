using System;

namespace Deveel.Workflows
{
    public sealed class ExecuteResult
    {
        private object result;
        private string errorMessage;

        public ExecuteResult(object result)
        {
            Result = result;
        }

        public ExecuteResult()
        {
        }

        public object Result {
            get { return result; }
            set
            {
                result = value;
                HasResult = true;
            }
        }

        public bool HasResult { get; private set; }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = null;
                HasError = true;
            }
        }

        public bool HasError { get; private set; }
    }
}
