using System;

using NUnit.Framework;

namespace Deveel.Workflows {
	[TestFixture]
	public class StateTests {
		[Test]
		public void TransportMeta() {
			var state = new State(89);
			state = state.New("first");
			state.SetMetadata("key1", 55);
			state = state.New("second");

			var meta = state.GetMetadata<int>("key1");
			Assert.AreEqual(55, meta);
		}

		[Test]
		public void OverrideMeta() {
			var state = new State(89);
			state = state.New("first");
			state.SetMetadata("key1", 55);
			state = state.New("second");
			state.SetMetadata("key1", "overridden");
			state = state.New("third");

			var meta = state.GetMetadata<string>("key1");
			Assert.AreEqual("overridden", meta);
		}
	}
}