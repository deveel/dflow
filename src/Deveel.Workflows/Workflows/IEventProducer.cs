using System;
using System.Collections.Generic;

namespace Deveel.Workflows
{
    public interface IEventProducer
    {
        IEnumerable<string> ProducedEvents { get; }
    }
}
