using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
						.Execute(state => state.SetValue((int) state.Value + 5))))
				.Merge("merge", values => values.Cast<int>().Sum(x => x)));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(28, final.Value);
			Assert.AreEqual("[begin]->seed->sbranch[addOne,addFive]->merge", final.PathString);
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

			Assert.IsInstanceOf<BranchState>(final);
			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsTrue(final.IsBranch);

			Assert.IsInstanceOf<IEnumerable<State>>(final.Value);
			Assert.AreEqual(2, final.AsBranch().Value.Length);
			Assert.AreEqual(23, final.AsBranch().Value[0]);
			Assert.AreEqual(27, final.AsBranch().Value[1]);
			Assert.AreEqual("[begin]->seed->pbranch[addOne|addFive]", final.PathString);
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
			Assert.AreEqual("[begin]->seed->pbranch[addOne|addFive]->merge", final.PathString);
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
				.Merge("merge", values => values.Cast<int>().Sum(x => x))
				.Repeat("checkRepeat", state => (int) state.Value < 100));

			var flow = builder.Build();

			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(100, final.Value);
		}

		[Test]
		public void UseTypedActivity() {
			var builder = Workflow.Build(workflow => workflow
				.Activity<AddTenActivity>()
				.Activity<AddTenActivity>());

			var flow = builder.Build();
			var final = flow.Execute(new State(23));

			Assert.IsNotNull(final);
			Assert.IsFalse(final.StateInfo.Failed);
			Assert.AreEqual(43, final.Value);
			Assert.AreEqual("[begin]->addTen->addTen", final.PathString);
		}

		[Test]
		public void MixBranchingAndMerge() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(11)))
				.Branch(branch => branch
					.Named("branch")
					.InParallel()
					.Branch(branch1 => branch1
						.Named("b1")
						.Activity<AddTenActivity>()
						.Activity<AddTenActivity>())      // here the value is 31
					.Branch(branch2 => branch2
						.Named("b2")
						.Activity<AddTenActivity>()
						.Activity<AddTenActivity>()))   // here the value is 31
				.Merge("merge", values => values.Cast<State[]>().Select(x => x.Sum(y => (int)y.Value)).Sum(x => x)));	// 31 + 31

			var flow = builder.Build();
			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsFalse(final.StateInfo.Failed);
			Assert.AreEqual(62, final.Value);
			Assert.AreEqual("[begin]->seed->branch[b1[addTen,addTen]|b2[addTen,addTen]]->merge", final.PathString);
		}

		[Test]
		public void BranchFactoryWithMerge() {
			var builder = Workflow.Build(workflow => workflow
				.Activity(activity => activity
					.Named("seed")
					.Execute(state => state.SetValue(11)))
				.Branch(branch => branch
					.Named("branch")
					.InParallel()
					.Branch(branch1 => branch1
						.Named("b1")
						.Activity<AddTenActivity>()
						.Activity<AddTenActivity>())
					.Branch(branch2 => branch2
						.Named("b2")
						.Activity<AddTenActivity>()
						.Activity<AddTenActivity>())
					.AsFactory(state => new[] {new State(state.Value), new State(state.Value)}))
				.Merge("merge",
					values => values.Cast<State[]>()
						.Select(x => x.Sum(y => ((State[])y.Value).Sum(z => (int)z.Value))).Sum(x => x)));

			var flow = builder.Build();
			var final = flow.Execute();

			Assert.IsNotNull(final);
			Assert.IsFalse(final.StateInfo.Failed);
			Assert.AreEqual(124, final.Value);
		}

		#region AddTenActivity

		class AddTenActivity : Activity<int, int> {
			public override string ActivityName {
				get { return "addTen"; }
			}

			protected override Task<int> ExecuteValueAsync(int input, CancellationToken cancellationToken) {
				return Task.FromResult(input + 10);
			}
		}

		#endregion
	}
}