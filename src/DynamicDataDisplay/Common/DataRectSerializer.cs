using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	public sealed class DataRectSerializer : ValueSerializer
	{
		public override bool CanConvertFromString(string value, IValueSerializerContext context)
		{
			return true;
		}

		public override bool CanConvertToString(object value, IValueSerializerContext context)
		{
			return value is DataRect;
		}

		public override object ConvertFromString(string value, IValueSerializerContext context)
		{
			if (value != null)
			{
				return DataRect.Parse(value);
			}
			return base.ConvertFromString(value, context);
		}

		public override string ConvertToString(object value, IValueSerializerContext context)
		{
			if (value is DataRect)
			{
				DataRect rect = (DataRect)value;
				return rect.ConvertToString(null, CultureInfo.GetCultureInfo("en-us"));
			}
			return base.ConvertToString(value, context);
		}
	}
}
