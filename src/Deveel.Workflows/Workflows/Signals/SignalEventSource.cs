using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Signals
{
    public sealed class SignalEventSource : EventSource
    {
        private ISignalRegistry registry;

        public SignalEventSource(ISignalRegistry registry)
        {
            this.registry = registry;
        }

        public override EventType EventType => EventType.Signal;

        protected override Task AttachContextAsync(EventContext context)
        {
            return registry.SubscribeAsync(context.Event.Name, new SignalHandler(context), context.CancellationToken);
        }

        protected override Task DetachContextAsync(EventContext context)
        {
            return registry.UnsubscribeAsync(context.Event.Name, context.EventId.ToString(), context.CancellationToken);
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
