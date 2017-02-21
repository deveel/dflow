using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Deveel.Workflows {
	[TestFixture]
	public class RegistryTests {
		[Test]
		public void CreateRegistry() {
			var registry = new WorkflowRegistry(new DefaultBuildContext());
			registry.Register("a", builder => builder
				.Activity<AddOneActivity>()
				.Activity<AddOneActivity>());

			var workflow = registry.GetWorkflow("a");

			Assert.IsNotNull(workflow);
		}

		#region AddOneActivity

		class AddOneActivity : Activity<int, int> {
			public override string ActivityName => "addOne";

			protected override Task<int> ExecuteValueAsync(int input, CancellationToken cancellationToken) {
				return Task.FromResult(input + 1);
			}
		}

		#endregion
	}
}