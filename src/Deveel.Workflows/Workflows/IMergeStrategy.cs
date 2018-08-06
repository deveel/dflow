using System;
using System.Threading.Tasks;

namespace Deveel.Workflows
{
    public interface IMergeStrategy
    {
        Task MergeAsync(NodeContext context);
    }
}
