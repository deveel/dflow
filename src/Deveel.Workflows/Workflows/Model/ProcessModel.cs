using System;
using Deveel.Workflows.Expressions;

namespace Deveel.Workflows.Model
{
    public class ProcessModel
    {
        public ProcessModel()
        {
            Sequence = new ProcessSequenceModel();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public ProcessSequenceModel Sequence { get; set; }

        public  EventModel StartEvent { get; set; }

        public EventModel EndEvent { get; set; }

        private Process BuildProcess(ModelBuildContext context)
        {
            if (String.IsNullOrWhiteSpace(Id))
                throw new InvalidOperationException();

            var process = new Process(new ProcessInfo(Id, context.InstanceKey));

            foreach (var model in Sequence)
            {
                var obj = model.BuildNode(context);
                process.Sequence.Add(obj);
            }

            return process;
        }

        public Process Build(IContext context, string instanceKey)
        {
            var processInfo = new ProcessInfo(Id, instanceKey);
            var buildContext = new ModelBuildContext(context, processInfo);

            var process = new Process(processInfo);

            foreach (var node in Sequence)
            {
                process.Sequence.Add(node.BuildNode(buildContext));
            }

            return process;
        }
    }
}
