using System;
using System.Threading.Tasks;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows
{
    public sealed class ActivityLoop : Activity
    {
        public ActivityLoop(Activity activity, FlowExpression condition)
            : base(activity.Id)
        {
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public Activity Activity { get; }

        public override FlowNodeType NodeType => Activity.NodeType;

        public FlowExpression Condition { get; }

        internal override async Task ExecuteNodeAsync(IExecutionContext context)
        {
            do
            {
                await Activity.ExecuteNodeAsync(context);
            } while (await Condition.IsTrueAsync(context));
        }
    }
}
