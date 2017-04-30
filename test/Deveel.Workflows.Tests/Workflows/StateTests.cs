using System;

using Xunit;

namespace Deveel.Workflows {
	public class StateTests {
		[Fact]
		public void TransportMeta() {
			var state = new State(89);
			state = state.New("first");
			state.SetMetadata("key1", 55);
			state = state.New("second");

			var meta = state.GetMetadata<int>("key1");
			Assert.Equal(55, meta);
		}

		[Fact]
		public void OverrideMeta() {
			var state = new State(89);
			state = state.New("first");
			state.SetMetadata("key1", 55);
			state = state.New("second");
			state.SetMetadata("key1", "overridden");
			state = state.New("third");

			var meta = state.GetMetadata<string>("key1");
			Assert.Equal("overridden", meta);
		}

		[Fact]
		public void TransportValue() {
			var state = new State("foo");
			state = state.New("first");
			state = state.New("second");

			Assert.NotNull(state.Value);
			Assert.False(state.HasNewValue);
			Assert.Equal("foo", state.Value);
		}

		[Fact]
		public void TransportNewValue() {
			var state = new State();
			state = state.New("first").SetValue("foo");

			Assert.True(state.HasNewValue);

			state = state.New("second");

			Assert.NotNull(state.Value);
			Assert.False(state.HasNewValue);
			Assert.Equal("foo", state.Value);
		}
	}
}