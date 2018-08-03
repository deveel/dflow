using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public class ProcessModel : ActivityModel
    {
        public ProcessModel()
        {
            Sequence = new ProcessSequenceModel();
        }

        public ProcessSequenceModel Sequence { get; set; }

        public  EventModel StartEvent { get; set; }

        public EventModel EndEvent { get; set; }

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

            var process = new Process(Id);

            foreach (var model in Sequence)
            {
                var obj = model.BuildNode(context);
                process.Sequence.Add(obj);
            }

            return process;
        }

        public Process Build(IContext context)
        {
            var buildContext = new ModelBuildContext(Id, context);
            return (Process) BuildNode(buildContext);
        }
    }
}
