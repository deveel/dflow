using Deveel.Workflows.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Messaging
{
    public interface IMessageReceiver : IDisposable
    {
        Task<Message> ReceiveAsync(MessageSubscription subscription, CancellationToken cancellationToken);
    }
}
