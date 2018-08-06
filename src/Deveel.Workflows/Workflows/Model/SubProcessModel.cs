using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public sealed class SubProcessModel : ActivityModel
    {
        public ProcessSequenceModel Sequence { get; set; }

        internal override Activity BuildActivity(ModelBuildContext context)
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
