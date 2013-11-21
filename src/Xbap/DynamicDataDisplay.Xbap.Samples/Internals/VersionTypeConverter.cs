using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Samples.Internals.Models;

namespace Microsoft.Research.DynamicDataDisplay.Samples.Internals
{
	public class VersionTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = (string)value;

			string[] parts = str.Split('.');

			int major = Int32.Parse(parts[0], culture);
			int minor = Int32.Parse(parts[1], culture);
			int revision = 0;
			if (parts.Length >= 3)
				revision = Int32.Parse(parts[2], culture);
			ReleaseVersion result = new ReleaseVersion(major, minor, revision);
			return result;
		}
	}
}
