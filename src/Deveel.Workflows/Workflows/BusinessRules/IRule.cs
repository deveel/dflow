using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deveel.Workflows.BusinessRules
{
    public interface IRule
    {
        string Name { get; }

        Task ExecuteAsync(IEnumerable<object> args, CancellationToken cancellationToken);
    }
}
