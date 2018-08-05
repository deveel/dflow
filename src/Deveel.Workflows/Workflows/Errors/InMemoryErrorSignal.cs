using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class InMemoryErrorSignal : IErrorSignal
    {
        private readonly Dictionary<ErrorKey, Task<ThrownError>> waiters;
        private readonly Dictionary<ErrorKey, ThrownError> errors;

        public InMemoryErrorSignal()
        {
            waiters = new Dictionary<ErrorKey, Task<ThrownError>>();
            errors = new Dictionary<ErrorKey, ThrownError>();
        }

        internal void OnErrorThrown(ThrownError error)
        {
            var key = new ErrorKey(error.ProcessId, error.InstanceId, error.Name);
            errors[key] = error;
        }

        private ThrownError WaitForError(ErrorKey key, CancellationToken cancellationToken)
        {
            ThrownError error = null;
            while (!errors.TryGetValue(key, out error))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            errors.Remove(key);
            return error;
        }

        public Task<ThrownError> WaitForErrorAsync(string processId, string instanceId, string errorName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var errorKey = new ErrorKey(processId, instanceId, errorName);

            if (!waiters.TryGetValue(errorKey, out Task<ThrownError> waitTask))
            {
                waitTask = Task.Run(() => WaitForError(errorKey, cancellationToken), cancellationToken);
                waiters[errorKey] = waitTask;
            }

            return waitTask;
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

            public override bool Equals(object obj)
            {
                if (!(obj is ErrorKey))
                    return false;

                return Equals((ErrorKey)obj);
            }

            public override int GetHashCode()
            {
                var hashCode = 663944616;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProcessId);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InstanceId);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ErrorName);
                return hashCode;
            }
        }

        #endregion
    }
}
