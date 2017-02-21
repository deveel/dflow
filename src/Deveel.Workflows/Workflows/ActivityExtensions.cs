using System;
using System.Globalization;
using System.Linq;

namespace Deveel.Workflows {
	public static class ActivityExtensions {
		public static T GetMetadata<T>(this IActivity activity, string key) {
			return activity.Metadata.GetValue<T>(key);
		}

		public static bool HasMetadata(this IActivity activity, string key) {
			return activity.Metadata.HasValue(key);
		}
	}
}