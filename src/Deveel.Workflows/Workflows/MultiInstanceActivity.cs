using System;
using System.Linq;
using System.Threading.Tasks;
using Deveel.Workflows.Expressions;
using Deveel.Workflows.Variables;

namespace Deveel.Workflows
{
    public class MultiInstanceActivity : Activity
    {
        public MultiInstanceActivity(Activity activity, FlowExpression countExpression, MultiInstanceType instanceType) 
            : base(activity.Id)
        {
            Activity = activity;
            CountExpression = countExpression;
            InstanceType = instanceType;
        }

        public Activity Activity { get; }

        public override FlowNodeType NodeType => Activity.NodeType;
        
        public MultiInstanceType InstanceType { get; }

        public FlowExpression CountExpression { get; }

        protected override async Task<object> CreateStateAsync(ExecutionContext context)
        {
            var state = await Activity.CallCreateStateAsync(context);
            var count = await CountExpression.ReduceToAsync<int>(context, context.CancellationToken);

            return new MultiInstanceState(state, count);
        }

        protected override Task ExecuteNodeAsync(object state, ExecutionContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var multiState = (MultiInstanceState) state;
            var tasks = new Action[multiState.Count];

            for (int i = 0; i < multiState.Count; i++)
            {
                tasks[i] = () => Activity.CallExecuteNodeAsync(multiState.State, context);
            }

            if (InstanceType == MultiInstanceType.Parallel)
            {
                return ExecuteParallel(context, tasks);
            } else if (InstanceType == MultiInstanceType.Sequential)
            {
                return ExecuteSequential(context, tasks);
            }

            throw new FlowException();
        }

        private async Task ExecuteParallel(ExecutionContext context, Action[] tasks)
        {
            int totalCount = tasks.Length;
            int completed = 0;
            int active = totalCount;

            await context.SetVariableAsync("nrOfInstances", totalCount);

            var wrapped = tasks.Select(async x =>
            {
                x();
                active--;
                completed--;

                await context.SetVariableAsync("nrOfActiveInstances", active);
                await context.SetVariableAsync("nrOfCompletedInstances", completed);
            }).ToArray();

            Task.WaitAll(wrapped, context.CancellationToken);
        }

        private async Task ExecuteSequential(ExecutionContext context, Action[] tasks)
        {
            int totalCount = tasks.Length;
            int completed = 0;
            int active = totalCount;

            await context.SetVariableAsync("nrOfInstances", totalCount);

            foreach (var task in tasks)
            {
                task();
                active--;
                completed++;

                await context.SetVariableAsync("nrOfActiveInstances", active);
                await context.SetVariableAsync("nrOfCompletedInstances", completed);
            }
        }

        class MultiInstanceState
        {
            public MultiInstanceState(object state, int count)
            {
                State = state;
                Count = count;
            }

            public int Count { get; }

            public object State { get; }
        }
    }
}
