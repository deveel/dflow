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

        protected override async Task<object> CreateStateAsync(ExecutionContext context)
        {
            var repository = context.GetRequiredService<IRulesProvider>();
            return await repository.FindRuleAsync(RuleId);
        }

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            var rule = (IRule) state;
            return rule.ExecuteAsync(context, CancellationToken.None);
        }
    }
}
