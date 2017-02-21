using System;

namespace Deveel.Workflows {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ActivityDecidesAttribute : ActivityMetadataAttribute {
		public ActivityDecidesAttribute() : this(true) {
		}

		public ActivityDecidesAttribute(bool value) : base(KnownActivityMetadataKeys.Decides, value) {
		}
	}
}