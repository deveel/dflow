using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Deveel.Workflows {
	static class MetadataUtils {
		public static bool HasValue(this IDictionary<string, object> metadata, string key) {
			return metadata?.ContainsKey(key) ?? false;
		}

		public static T GetValue<T>(this IEnumerable<KeyValuePair<string, object>> metadata, string key) {
			if (metadata == null)
				return default(T);

			return metadata.ToDictionary(x => x.Key, y => y.Value, StringComparer.Ordinal)
				.GetValue<T>(key);
		}

		public static T GetValue<T>(this IDictionary<string, object> metadata, string key) {
			if (metadata == null)
				return default(T);

			object obj;
			if (!metadata.TryGetValue(key, out obj))
				return default(T);

			if (!(obj is T)) {
				var nullableType = Nullable.GetUnderlyingType(typeof(T));
				if (nullableType == null) {
					obj = Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
				} else {
					obj = Convert.ChangeType(obj, nullableType, CultureInfo.InvariantCulture);
				}
			}

			return (T)obj;
		}
	}
}