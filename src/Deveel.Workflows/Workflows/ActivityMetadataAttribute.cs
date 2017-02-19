using System;

namespace Deveel.Workflows {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ActivityMetadataAttribute : Attribute {
		public ActivityMetadataAttribute(string key, object value) {
			if (String.IsNullOrEmpty(key))
				throw new ArgumentNullException(nameof(key));

			Key = key;
			Value = value;
		}

		public string Key { get; }

		public object Value { get; }
	}
}