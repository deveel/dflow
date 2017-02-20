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

		[Test]
		public void TransportValue() {
			var state = new State("foo");
			state = state.New("first");
			state = state.New("second");

			Assert.IsNotNull(state.Value);
			Assert.IsFalse(state.HasNewValue);
			Assert.AreEqual("foo", state.Value);
		}

		[Test]
		public void TransportNewValue() {
			var state = new State();
			state = state.New("first").SetValue("foo");

			Assert.IsTrue(state.HasNewValue);

			state = state.New("second");

			Assert.IsNotNull(state.Value);
			Assert.IsFalse(state.HasNewValue);
			Assert.AreEqual("foo", state.Value);
		}
	}
}