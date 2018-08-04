using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.BusinessRules
{
    public interface IRulesProvider
    {
        string Name { get; }

        Task<IRule> FindRuleAsync(string name);
    }
}
