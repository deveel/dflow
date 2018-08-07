using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Errors
{
    public sealed class InMemoryErrorHandler : IErrorSignalHandler
    {
        private readonly Dictionary<ErrorKey, Task<ThrownError>> waiters;
        private readonly Dictionary<ErrorKey, ThrownError> errors;

        public InMemoryErrorHandler()
        {
            waiters = new Dictionary<ErrorKey, Task<ThrownError>>();
            errors = new Dictionary<ErrorKey, ThrownError>();
        }

        Task IErrorSignalHandler.SignalAsync(ThrownError error, CancellationToken cancellationToken)
        {
            var key = new ErrorKey(error.ProcessId, error.InstanceId, error.Code);
            errors[key] = error;
            return Task.CompletedTask;
        }

        private ThrownError WaitForError(ErrorKey key, CancellationToken cancellationToken)
        {
            ThrownError error;
            while (!errors.TryGetValue(key, out error))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            errors.Remove(key);
            return error;
        }

        public Task<ThrownError> CatchErrorAsync(string processId, string instanceId, string errorCode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var errorKey = new ErrorKey(processId, instanceId, errorCode);

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
            public ErrorKey(string processId, string instanceId, string errorCode)
            {
                ProcessId = processId;
                InstanceId = instanceId;
                ErrorCode = errorCode;
            }

            public string ProcessId { get; }

            public string InstanceId { get; }

            public string ErrorCode { get; }

            public bool Equals(ErrorKey other) {
                if (other == null)
                    return false;

                return ProcessId == other.ProcessId &&
                    InstanceId == other.InstanceId &&
                    ErrorCode == other.ErrorCode;
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
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ErrorCode);
                return hashCode;
            }
        }

        #endregion
    }
}
