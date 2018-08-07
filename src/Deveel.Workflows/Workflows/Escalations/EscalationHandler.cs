using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Escalations {
    public sealed class EscalationHandler : IDisposable {
        private Dictionary<string, Action> handlers;
        private Dictionary<string, AutoResetEvent> waiters;
        private AutoResetEvent allWaiter;
        private Action catchAll;

        public EscalationHandler() {
            handlers = new Dictionary<string, Action>();
            waiters = new Dictionary<string, AutoResetEvent>();
            allWaiter = new AutoResetEvent(false);
        }

        public void Handle(string code, Action handler) {
            if (String.IsNullOrWhiteSpace(code)) {
                catchAll = (Action) Delegate.Combine(catchAll, handler);
            }
            else {
                if (handlers.TryGetValue(code, out var catchIt)) {
                    handlers[code] = (Action) Delegate.Combine(catchIt, handler);
                    waiters[code] = new AutoResetEvent(false);
                }
                else {
                    handlers[code] = handler;
                }
            }
        }

        public void Unhandle(string code) {
            if (String.IsNullOrWhiteSpace(code)) {
                catchAll = null;
            }
            else {
                if (handlers.Remove(code)) {
                    var waiter = waiters[code];
                    waiter.Dispose();

                    waiters.Remove(code);
                }
            }
        }

        public void Signal(string code) {
            if (String.IsNullOrWhiteSpace(code)) {
                allWaiter.Set();
            }
            else if (waiters.TryGetValue(code, out AutoResetEvent waiter)) {
                waiter.Set();
            }
        }

        public Task CatchAsync(string code, CancellationToken cancellationToken) {
            AutoResetEvent waiter;

            if (String.IsNullOrWhiteSpace(code))
                waiter = allWaiter;
            else if (!waiters.TryGetValue(code, out waiter))
                return Task.CompletedTask;

            return Task.Run(() => waiter.WaitOne(), cancellationToken);
        }

        public void Dispose() {
            foreach (var waiter in waiters.Values) {
                waiter?.Dispose();
            }

            allWaiter?.Dispose();
        }
    }
}