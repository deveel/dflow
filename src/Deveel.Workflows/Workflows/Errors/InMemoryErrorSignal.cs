using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class InMemoryErrorSignal : IErrorSignal
    {
        private readonly Dictionary<string, Task<ThrownError>> waiters;
        private readonly Dictionary<string, ThrownError> errors;

        public InMemoryErrorSignal()
        {
            waiters = new Dictionary<string, Task<ThrownError>>();
            errors = new Dictionary<string, ThrownError>();
        }

        internal void OnErrorThrown(ThrownError error)
        {
            errors[error.Name] = error;
        }

        public Task<ThrownError> WaitForErrorAsync(string errorName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!waiters.TryGetValue(errorName, out Task<ThrownError> waitTask))
            {
                waitTask = Task.Run(() => WaitForError(errorName, cancellationToken), cancellationToken);
                waiters[errorName] = waitTask;
            }

            return waitTask;
        }

        private ThrownError WaitForError(string errorName, CancellationToken cancellationToken)
        {
            ThrownError error = null;
            while (!errors.TryGetValue(errorName, out error))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            errors.Remove(errorName);
            return error;
        }

        #region ErrorKey

        class ErrorKey : IEquatable<ErrorKey>
        {
            public ErrorKey(string processId, string instanceId, string errorName)
            {
                ProcessId = processId;
                InstanceId = instanceId;
                ErrorName = errorName;
            }

            public string ProcessId { get; }

            public string InstanceId { get; }

            public string ErrorName { get; }

            public bool Equals(ErrorKey other)
            {
                return ProcessId == other.ProcessId &&
                    InstanceId == other.InstanceId &&
                    ErrorName == other.ErrorName;
            }
        }

        #endregion
    }
}
