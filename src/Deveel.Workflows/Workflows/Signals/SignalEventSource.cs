using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public sealed class SignalEventSource : EventSource
    {
        private ISignalRegistry registry;

        public SignalEventSource(ISignalRegistry registry, string id, string signalName)
            : base(id) {
            this.registry = registry;
            SignalName = signalName;
        }

        public string SignalName { get; }

        public override EventType EventType => EventType.Signal;

        protected override Task AttachContextAsync(EventContext context)
        {
            return registry.SubscribeAsync(SignalName, new SignalHandler(context), context.CancellationToken);
        }

        protected override Task DetachContextAsync(EventContext context)
        {
            return registry.UnsubscribeAsync(SignalName, context.EventId.ToString(), context.CancellationToken);
        }

        #region SignalHandler

        class SignalHandler : ISignalHandler
        {
            private EventContext context;

            public SignalHandler(EventContext context)
            {
                this.context = context;
            }

            public string Id => context.EventId.ToString();

            public Task HandleAsync(Signal signal, CancellationToken cancellationToken)
            {
                return context.FireAsync(signal);
            }
        }

        #endregion
    }
}
