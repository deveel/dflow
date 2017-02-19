using System;

namespace Deveel.Workflows {
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ActivityNameAttribute : ActivityMetadataAttribute {
		public ActivityNameAttribute(string name)
			: base(KnownActivityMetadataKeys.Name, name) {
		}
	}
}