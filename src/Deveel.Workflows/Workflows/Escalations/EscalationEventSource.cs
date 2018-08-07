using System;
using System.Threading.Tasks;

using Deveel.Workflows.Events;

namespace Deveel.Workflows.Escalations
{
    public sealed class EscalationEventSource : EventSource {
        private readonly EscalationHandler handler;

        public EscalationEventSource(EscalationHandler handler, string id, string code)
            : base(id) {
            Code = code;
            this.handler = handler;
        }

        public override EventType EventType => EventType.Escalation;

        public string Code { get; }

        protected override Task AttachContextAsync(EventContext context) {
            handler.Handle(Code, async () => {
                await context.FireAsync(new Escalation(context.EventId.ProcessId, context.EventId.InstanceKey, Code));
            });
            return Task.CompletedTask;
        }

        protected override Task DetachContextAsync(EventContext context) {
            handler.Unhandle(Code);
            return Task.CompletedTask;
        }
    }
}
