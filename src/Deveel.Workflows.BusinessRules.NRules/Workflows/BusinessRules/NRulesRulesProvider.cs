using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NRules.Fluent;
using NRules.RuleModel;

namespace Deveel.Workflows.BusinessRules
{
    public sealed class NRulesRulesProvider : IRulesProvider
    {
        private readonly Dictionary<string, IRuleRepository> repositories;

        public NRulesRulesProvider()
        {
            repositories = new Dictionary<string, IRuleRepository>();
        }

        public string Name => "nrules";

        public void AddRules(string name, IRuleRepository repository)
        {
            repositories.Add(name, repository);
        }

        public void AddRules(string name, params Type[] types)
        {
            var repository = new RuleRepository();
            repository.Load(x => x.From(types));
            AddRules(name, repository);
        }

        public Task<IRule> FindRuleAsync(string name)
        {
            if (!repositories.TryGetValue(name, out var repository))
                return Task.FromResult<IRule>(null);

            return Task.FromResult<IRule>(new NRulesRule(name, repository));
        }
    }
}
