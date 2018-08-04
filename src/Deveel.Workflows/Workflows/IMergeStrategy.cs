using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public interface IMergeStrategy
    {
        Task MergeAsync(ExecutionContext context);
    }
}
