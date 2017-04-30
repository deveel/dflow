using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Deveel.Workflows.Graph;

using Xunit;

namespace Deveel.Workflows {
	public class GraphTests {
		[Fact]
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
			Assert.NotNull(graph.Nodes);

			var nodes = graph.Nodes.ToList();
			Assert.Equal(2, nodes.Count);
			Assert.Null(nodes[0].Previous);
			Assert.NotNull(nodes[0].Next);
			Assert.Equal("a", nodes[0].Name);
			Assert.NotNull(nodes[1]);
			Assert.NotNull(nodes[1].Previous);
			Assert.Null(nodes[1].Next);
		}

		[Fact]
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

			Assert.True(nodes[1].HasDecision);
		}

		[Fact]
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

			Assert.Equal(3, nodes.Count);
			Assert.NotEmpty(nodes[2].Nodes);
			Assert.NotNull(nodes[2].Nodes.First());
		}

		[Fact]
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

			Assert.NotNull(graph);
			Assert.NotEmpty(graph.Nodes);

			var nodes = graph.Nodes.ToList();

			Assert.Equal(3, nodes.Count);
			Assert.NotNull(nodes[0]);
			Assert.NotNull(nodes[2]);
			Assert.NotEmpty(nodes[2].Nodes);
		}

		[Fact]
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