using System;
using System.Globalization;
using System.Linq;

namespace Deveel.Workflows {
	public static class ActivityExtensions {
		public static T GetMetadata<T>(this IActivity activity, string key) {
			if (activity.Metadata == null)
				return default(T);

			var dict = activity.Metadata.ToDictionary(x => x.Key, y => y.Value, StringComparer.Ordinal);
			object obj;

			if (!dict.TryGetValue(key, out obj))
				return default(T);

			if (!(obj is T)) {
				var nullableType = Nullable.GetUnderlyingType(typeof(T));
				if (nullableType == null) {
					obj = Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
				} else {
					obj = Convert.ChangeType(obj, nullableType, CultureInfo.InvariantCulture);
				}
			}

			return (T) obj;
		}
	}
}