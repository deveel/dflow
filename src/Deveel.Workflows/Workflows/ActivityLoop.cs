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

        protected override Task<object> CreateStateAsync(NodeContext context)
        {
            return Activity.CallCreateStateAsync(context);
        }

        protected override async Task ExecuteActivityAsync(ActivityContext context, object state)
        {
            do
            {
                await Activity.CallExecuteNodeAsync(state, context);
            } while (await Condition.IsTrueAsync(context, context.CancellationToken));
        }
    }
}
