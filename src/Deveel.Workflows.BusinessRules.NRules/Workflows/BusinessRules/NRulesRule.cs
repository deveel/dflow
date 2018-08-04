using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.Variables;
using NRules;
using NRules.RuleModel;

namespace Deveel.Workflows.BusinessRules
{
    public sealed class NRulesRule : IRule
    {
        public NRulesRule(string name, IRuleRepository repository)
        {
            Name = name;

            var factory = repository.Compile();
            session = factory.CreateSession();
        }

        private ISession session;

        public string Name { get; }

        public async Task ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            var variables = await context.GetVariablesAsync();

            session.InsertAll(variables);
            session.Fire();
        }
    }
}
