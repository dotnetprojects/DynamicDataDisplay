using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class StringExtensions
	{
		public static string Format(this string formatString, object param)
		{
			return String.Format(formatString, param);
		}

		public static string Format(this string formatString, object param1, object param2)
		{
			return String.Format(formatString, param1, param2);
		}
	}
}
