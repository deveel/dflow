using System;

namespace Deveel.Workflows {
	[AttributeUsage(AttributeTargets.Class)]
	public class ActivityDescriptionAttribute : ActivityMetadataAttribute {
		public ActivityDescriptionAttribute(string text)
			: base(KnownActivityMetadataKeys.Description, text) {
		}
	}
}