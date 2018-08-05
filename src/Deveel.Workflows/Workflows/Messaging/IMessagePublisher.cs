using System;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.Messaging
{
    public interface IMessagePublisher
    {
        Task<IPublishResponse> PusblishAsync(Message message, PublishBehavior behavior, CancellationToken cancellationToken);
    }
}
