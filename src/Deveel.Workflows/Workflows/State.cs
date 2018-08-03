using System;

namespace Deveel.Workflows
{
    public sealed class State
    {
        public State(object inputValue) 
            : this(inputValue, null)
        {
        }

        public State(object inputValue, State previous)
        {
            InputValue = inputValue;
            Previous = previous;
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public State(State previous)
            : this(null, previous)
        {
        }

        public object InputValue { get; }

        public DateTimeOffset TimeStamp { get; }

        public State Previous { get; }

        public bool HasResult { get; private set; }

        public object Result { get; private set; }

        public Exception Error { get; private set; }

        public bool HasError { get; private set; }

        public State SetResult(object result)
        {
            HasResult = true;
            Result = result;
            return this;
        }

        public State Terminate()
        {
            return SetResult(null);
        }

        public State SetError(Exception error)
        {
            Error = error;
            HasError = true;
            return this;
        }

        public State Next()
        {
            return new State(InputValue, this);
        }
    }
}
