using System;
using System.Collections.Generic;
using System.Linq;

namespace Deveel.Workflows.Graph {
	class ComponentNode : ExecutionNode {
		public ComponentNode(IComponent component) {
			if (component == null)
				throw new ArgumentNullException(nameof(component));

			Component = component;
			HasDecision = component is IActivity && ((IActivity) component).HasDecision;
		}

		private IComponent Component { get; }

		public override string Name => Component.Name;

		public override bool HasDecision { get; }

		public override IDictionary<string, object> Metadata => GetMetadata();

		private IDictionary<string, object> GetMetadata() {
			var activity = Component as IActivity;
			if (activity == null)
				return new Dictionary<string, object>();

			return activity.Metadata.ToDictionary(x => x.Key, y => y.Value);
		}

		public override IEnumerable<IExecutionNode> Nodes => FindInnerNodes();

		private IEnumerable<IExecutionNode> FindInnerNodes() {
			var branch = Component as IBranch;
			if (branch == null)
				return new IExecutionNode[0];

			return BuildNodes(branch.Activities);
		}

		public static IEnumerable<ComponentNode> BuildNodes(IEnumerable<IComponent> components) {
			var list = components.Select(x => new ComponentNode(x)).ToList();
			return list.Select((node, offset) => {
				var next = offset + 1 >= list.Count ? null : list[offset + 1];
				var previous = offset - 1 < 0 ? null : list[offset - 1];

				if (next != null)
					node.SetNext(next);
				if (previous != null)
					node.SetPrevious(previous);

				return node;
			});
		}
	}
}