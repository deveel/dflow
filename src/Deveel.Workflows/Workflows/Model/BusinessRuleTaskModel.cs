using System;
using System.Collections.Generic;
using System.Text;

namespace Deveel.Workflows.Model
{
    public sealed class BusinessRuleTaskModel : TaskModel
    {
        public BusinessRuleTaskModel()
        {
            VariableNames = new List<string>();
        }

        public string RuleName { get; }

        public ICollection<string> VariableNames { get; set; }

        internal override Activity BuildActivity(ModelBuildContext context)
        {
            return new BusinessRuleTask(Id, RuleName)
            {
                VariableNames = VariableNames
            };
        }
    }
}
