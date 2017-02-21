using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Deveel.Workflows {
	[TestFixture]
	public class SimpleWorkflowTests {
		[Test]
		public async Task CreateAndExecuteEcho() {
			string echo = null;

			var workflow = new Workflow {
				{
					"echo", state => state.SetValue(echo = state.Value.ToString())
				}
			};

			var initial = new State("Hello World!");
			var final = await workflow.ExecuteAsync(initial);

			Assert.IsNotNull(echo);
			Assert.AreEqual("Hello World!", echo);
			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<string>(final.Value);
			Assert.AreEqual("Hello World!", final.Value);
			Assert.AreEqual("[begin]->echo", final.PathString);
		}

		[Test]
		public async Task SimpleTransform() {
			var workflow = new Workflow {
				{
					"hello", state => state.SetValue("Hello")
				}, {
					"world", state => state.SetValue(state.Value.ToString() + " World!")
				}
			};

			var final = await workflow.ExecuteAsync(new State());

			Assert.IsNotNull(final);
			Assert.IsInstanceOf<string>(final.Value);
			Assert.AreEqual("Hello World!", final.Value);
			Assert.AreEqual("[begin]->hello->world", final.PathString);
		}

		[Test]
		public async Task ConditionalTransform() {
			var workflow = new Workflow {
				{
					"hello", state => state.SetValue("Hello")
				}, {
					"world", state => state.SetValue(state.Value.ToString() + " World!")
				}, {
					"world2",
					state => state.Value.ToString() != "Hello World!",
					state => state.New("world2").SetValue(state.Value)
				}
			};

			var final = await workflow.ExecuteAsync(new State());

			Assert.IsNotNull(final);
			Assert.IsInstanceOf<string>(final.Value);
			Assert.AreEqual("Hello World!", final.Value);
			Assert.AreEqual("[begin]->hello->world->world2", final.PathString);
			Assert.IsFalse(final.StateInfo.Executed);
		}

		[Test]
		public async Task SequentialBranch() {
			var workflow = new Workflow {
				{
					"seed", state => state.SetValue(22)
				}, {
					"sbranch", BranchStrategies.Sequential,
					new Activity("addOne", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 1))),
					new Activity("addFive", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 5)))
				},
				{"merge", MergeStrategies.New(values => values.Cast<int>().Sum(x => x))}
			};

			var final = await workflow.ExecuteAsync();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(28, final.Value);
			Assert.AreEqual("[begin]->seed->sbranch[addOne,addFive]->merge", final.PathString);
		}

		[Test]
		public async Task UnmergedParallelBranch() {
			var workflow = new Workflow {
				{
					"seed", state => state.SetValue(22)
				}, {
					"pbranch", BranchStrategies.Parallel,
					new Activity("addOne", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 1))),
					new Activity("addFive", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 5)))
				}
			};

			var final = await workflow.ExecuteAsync();

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
		public async Task MergedParallelBranch() {
			var workflow = new Workflow {
				{
					"seed", state => state.SetValue(22)
				}, {
					"pbranch", BranchStrategies.Parallel,
					new Activity("addOne", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 1))),
					new Activity("addFive", (state, token) => Task.FromResult( state.SetValue((int) state.Value + 5)))
				},
				{"merge", MergeStrategies.New((IEnumerable<int> inputs) => inputs.Sum(x => x))}
			};

			var final = await workflow.ExecuteAsync(new State());

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(50, final.Value);
			Assert.AreEqual("[begin]->seed->pbranch[addOne|addFive]->merge", final.PathString);
		}

		[Test]
		public async Task RepeatBranch() {
			var workflow = new Workflow {
				{
					"seed", state => state.SetValue(22)
				}, {
					"sbranch", BranchStrategies.Sequential,
					new Activity("addOne", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 1))),
					new Activity("addFive", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 5)))
				},
				{"merge", MergeStrategies.New(values => values.Cast<int>().Sum(x => x))},
				{"checkRepeat", RepeatDecision.New(state => (int) state.Value < 100)}
			};

			var final = await workflow.ExecuteAsync();

			Assert.IsNotNull(final);
			Assert.IsNotNull(final.Value);
			Assert.IsInstanceOf<int>(final.Value);
			Assert.AreEqual(100, final.Value);
		}

		[Test]
		public async Task ReportTests() {
			var workflow = new Workflow {
				{
					"seed", state => state.SetValue(22)
				}, {
					"sbranch", BranchStrategies.Sequential,
					new Activity("addOne", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 1))),
					new Activity("addFive", (state, token) => Task.FromResult(state.SetValue((int) state.Value + 5)))
				},
				{"merge", MergeStrategies.New(values => values.Cast<int>().Sum(x => x))},
				{"checkRepeat", RepeatDecision.New(state => (int) state.Value < 100)}
			};

			var final = await workflow.ExecuteAsync();

			var report = final.GetReport();

			Assert.IsNotNull(report);
			Assert.IsNotEmpty(report.Nodes);

			var nodes = report.Nodes.ToList();
			Assert.AreEqual(4, nodes.Count);

			Assert.AreEqual("seed", nodes[0].ComponentName);
			Assert.IsTrue(nodes[1].Branch);
		}
	}
}