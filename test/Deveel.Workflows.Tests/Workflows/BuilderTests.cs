using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Deveel.Workflows {
	[TestFixture]
	public class BuilderTests {
		[Test]
		public async Task BuildSimpleEchoWorkflowAndExecute() {
			string echo = null;

			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("echo")
					.Execute(state => state.SetValue(echo = "Hello World"))));

			var flow = builder.Build();

			Assert.IsNotNull(flow);
			Assert.AreEqual(1, flow.Activities.Count());

			var result = await flow.ExecuteAsync();

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Value);
			Assert.AreEqual(echo, result.Value);
		}

		[Test]
		public void SimpleTransform() {
			var builder = Workflow.Build(worflow => worflow
				.Activity(activity => activity
					.Named("hello")
					.Execute(state => state.SetValue("Hello")))
				.Activity(activity => activity
					.Named("world")
					.If(state => (string) state.Value == "Hello")
					.Execute(state => state.SetValue((string) state.Value + " World!"))));

			var flow = builder.Build();

			Assert.IsNotNull(flow);
			Assert.AreEqual(2, flow.Activities.Count());

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsInstanceOf<string>(final.Value);
			Assert.AreEqual("Hello World!", final.Value);
			Assert.AreEqual("[begin]->hello->world", final.PathString);
		}

		[Test]
		public void ConditionalTransform() {
			var builder = Workflow.Build(worflow => worflow
				.Activity(activity => activity
					.Named("hello")
					.Execute(state => state.SetValue("Hello")))
				.Activity(activity => activity
					.Named("world")
					.If(state => (string) state.Value == "Hello")
					.Execute(state => state.SetValue((string) state.Value + " World!")))
				.Activity(activity => activity
					.Named("world2")
					.If(state => (string) state.Value == "Hello")
					.Execute(state => state.SetValue((string) state.Value + " World!"))));

			var flow = builder.Build();

			Assert.IsNotNull(flow);
			Assert.AreEqual(3, flow.Activities.Count());

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsInstanceOf<string>(final.Value);
			Assert.AreEqual("Hello World!", final.Value);
			Assert.AreEqual("[begin]->hello->world->world2", final.PathString);
			Assert.IsFalse(final.StateInfo.Executed);
		}

		[Test]
		public void SequentialBranch() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(22)))
				.Branch(branch => branch
					.Named("sbranch")
					.InSequence()
					.Activity(activity => activity
						.Named("addOne")
						.Execute(state => state.SetValue((int) state.Value + 1)))
					.Activity(activity => activity
						.Named("addFive")
						.Execute(state => state.SetValue((int) state.Value + 5)))));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(28, final.Value);
			Assert.AreEqual("[begin]->seed->sbranch->addOne->addFive", final.PathString);
		}

		[Test]
		public void UnmergedParallelBranch() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(22)))
				.Branch(branch => branch
					.Named("pbranch")
					.InParallel()
					.Activity(activity => activity
						.Named("addOne")
						.Execute(state => state.SetValue((int) state.Value + 1)))
					.Activity(activity => activity
						.Named("addFive")
						.Execute(state => state.SetValue((int) state.Value + 5)))));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsInstanceOf<ParallelState>(final);
			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsTrue(final.IsParallel);

			Assert.IsInstanceOf<IEnumerable<State>>(final.Value);
			Assert.AreEqual(2, final.AsParallel().Object.Length);
			Assert.AreEqual(23, final.AsParallel().Object[0]);
			Assert.AreEqual(27, final.AsParallel().Object[1]);
			Assert.AreEqual("[begin]->seed->pbranch[addOne,addFive]", final.PathString);
		}

		[Test]
		public void MergedParallelBranch() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(22)))
				.Branch(branch => branch
					.Named("pbranch")
					.InParallel()
					.Activity(activity => activity
						.Named("addOne")
						.Execute(state => state.SetValue((int) state.Value + 1)))
					.Activity(activity => activity
						.Named("addFive")
						.Execute(state => state.SetValue((int) state.Value + 5))))
				.Merge("merge", array => array.Cast<int>().Sum(x => x)));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(50, final.Value);
			Assert.AreEqual("[begin]->seed->pbranch[addOne,addFive]->merge", final.PathString);
		}

		[Test]
		public void RepeatBranch() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(22)))
				.Branch(branch => branch
					.Named("sbranch")
					.Activity(activity => activity
						.Named("addOne")
						.Execute(state => state.SetValue((int) state.Value + 1)))
					.Activity(activity => activity
						.Named("addFive")
						.Execute(state => state.SetValue((int) state.Value + 5))))
				.Repeat("checkRepeat", state => (int)state.Value < 100));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(100, final.Value);
		}
	}
}