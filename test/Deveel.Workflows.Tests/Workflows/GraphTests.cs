using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Deveel.Workflows.Graph;

using NUnit.Framework;

namespace Deveel.Workflows {
	[TestFixture]
	public class GraphTests {
		[Test]
		public void SimpleSequentialGraph() {
			var workflow = Workflow.Build(builder => builder
				.Activity(x => x
					.Named("a")
					.Execute(state => state.SetValue("foo")))
				.Activity(x => x
					.Named("b")
					.Execute(state => state.SetValue((string) state.Previous.Value + " bar"))));

			var flow = workflow.Build();

			var graph = flow.Graph();
			Assert.IsNotNull(graph.Nodes);

			var nodes = graph.Nodes.ToList();
			Assert.AreEqual(2, nodes.Count);
			Assert.IsNull(nodes[0].Previous);
			Assert.IsNotNull(nodes[0].Next);
			Assert.AreEqual("a", nodes[0].Name);
			Assert.IsNotNull(nodes[1]);
			Assert.IsNotNull(nodes[1].Previous);
			Assert.IsNull(nodes[1].Next);
		}

		[Test]
		public void NodeWithDecision() {
			var workflow = Workflow.Build(builder => builder
				.Activity(x => x
					.Named("a")
					.Execute(state => state.SetValue("foo")))
				.Activity(x => x
					.Named("b")
					.If(state => state.Value is string)
					.Execute(state => state.SetValue((string)state.Previous.Value + " bar"))));

			var flow = workflow.Build();

			var graph = flow.Graph();
			var nodes = graph.Nodes.ToList();

			Assert.IsTrue(nodes[1].HasDecision);
		}

		[Test]
		public void BranchNode() {
			var workflow = Workflow.Build(builder => builder
				.Activity(x => x
					.Named("a")
					.Execute(state => state.SetValue("foo")))
				.Activity(x => x
					.Named("b")
					.If(state => state.Value is string)
					.Execute(state => state.SetValue((string) state.Previous.Value + " bar")))
				.Branch(branch => branch.Named("b1")
					.Activity(x => x
						.Named("a")
						.Execute(state => state.SetValue(state.Previous.Value)))));

			var flow = workflow.Build();

			var graph = flow.Graph();
			var nodes = graph.Nodes.ToList();

			Assert.AreEqual(3, nodes.Count);
			Assert.IsNotEmpty(nodes[2].Nodes);
			Assert.IsNotNull(nodes[2].Nodes.First());
		}

		[Test]
		public void ProjectBuilder() {
			var workflow = Workflow.Build(builder => builder
				.Activity(x => x
					.Named("a")
					.Execute(state => state.SetValue("foo")))
				.Activity(x => x
					.Named("b")
					.If(state => state.Value is string)
					.Execute(state => state.SetValue((string)state.Previous.Value + " bar")))
				.Branch(branch => branch.Named("b1")
					.Activity(x => x
						.Named("a")
						.Execute(state => state.SetValue(state.Previous.Value)))));

			var graph = workflow.Graph();

			Assert.IsNotNull(graph);
			Assert.IsNotEmpty(graph.Nodes);

			var nodes = graph.Nodes.ToList();

			Assert.AreEqual(3, nodes.Count);
			Assert.IsNotNull(nodes[0]);
			Assert.IsNotNull(nodes[2]);
			Assert.IsNotEmpty(nodes[2].Nodes);
		}

		[Test]
		public void InspectMeta() {
			var workflow = Workflow.Build(builder => builder
				.Activity(x => x
					.Named("a")
					.Execute(state => state.SetValue("foo")))
				.Activity(x => x
					.Named("b")
					.If(state => state.Value is string)
					.Execute(state => state.SetValue((string) state.Previous.Value + " bar")))
				.Branch(branch => branch.Named("b1")
					.Activity(x => x
						.Named("a")
						.Execute(state => state.SetValue(state.Previous.Value)))));

			var graph = workflow.Graph().Nodes.ToList();
		}
	}
}