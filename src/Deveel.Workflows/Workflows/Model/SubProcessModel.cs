using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public sealed class SubProcessModel : ActivityModel
    {
        public ProcessSequenceModel Sequence { get; set; }

        internal override FlowNode BuildNode(ModelBuildContext context)
        {
            var process = BuildProcess(context);

            if (LoopCondition != null)
                process = new ActivityLoop(process, FlowExpression.Parse(LoopCondition));

            return process;
        }

        private Activity BuildProcess(ModelBuildContext context)
        {
            if (String.IsNullOrWhiteSpace(Id))
                throw new InvalidOperationException();

            var process = new SubProcess(Id);

            foreach (var model in Sequence)
            {
                var obj = model.BuildNode(context);
                process.Sequence.Add(obj);
            }

            return process;
        }

    }
}
