using System;
using System.Threading;
using System.Threading.Tasks;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Escalations {
    public sealed class EscalationRise : IEventRise {
        private readonly EscalationHandler handler;

        public EscalationRise(EscalationHandler handler) {
            this.handler = handler;
        }

        EventType IEventRise.EventType => EventType.Escalation;

        Task IEventRise.FireAsync(IEventArgument arg, CancellationToken cancellationToken) {
            handler.Signal(((Escalation) arg).Code);

            return Task.CompletedTask;
        }
    }
}