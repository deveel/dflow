using System;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.BusinessRules;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows
{
    public sealed class BusinessRuleTask : TaskBase
    {
        public BusinessRuleTask(string id, string ruleId) : base(id)
        {
            RuleId = ruleId;
        }

        public string RuleId { get; }

        private IRule Rule { get; set; }

        public override async Task ExecuteAsync(IExecutionContext context)
        {
            var repository = context.GetRequiredService<IRulesProvider>();
            Rule = await repository.FindRuleAsync(RuleId);

            await base.ExecuteAsync(context);
        }

        internal override Task ExecuteNodeAsync(IExecutionContext context)
        {
            return Rule.ExecuteAsync(context, CancellationToken.None);
        }
    }
}
