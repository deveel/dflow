using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Deveel.Workflows.BusinessRules;
using Microsoft.Extensions.DependencyInjection;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows
{
    public sealed class BusinessRuleTask : TaskBase
    {
        public BusinessRuleTask(string id, string ruleId) : base(id)
        {
            RuleId = ruleId;
            VariableNames = new List<string>();
        }

        public string RuleId { get; }

        public ICollection<string> VariableNames { get; set; }

        protected override async Task<object> CreateStateAsync(NodeContext context)
        {
            var repository = context.GetRequiredService<IRulesProvider>();
            return await repository.FindRuleAsync(RuleId);
        }

        protected override async Task ExecuteNodeAsync(object state, NodeContext context)
        {
            var rule = (IRule) state;

            List<object> args;

            if (VariableNames != null)
            {
                args = new List<object>();

                foreach(var varName in VariableNames)
                {
                    if (context.TryGetVariable(varName, out Variable variable))
                        args.Add(variable.Value);
                }
            } else
            {
                args = (await context.GetVariablesAsync()).Select(x => x.Value).ToList();
            }

            await rule.ExecuteAsync(args, context.CancellationToken);
        }
    }
}
