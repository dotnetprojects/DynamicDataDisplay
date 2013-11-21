using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Common
{
	internal static class TokenizerHelper
	{
		public static char GetNumericListSeparator(IFormatProvider provider)
		{
			char separator = ',';

			NumberFormatInfo numberInfo = NumberFormatInfo.GetInstance(provider);
			if (numberInfo.NumberDecimalSeparator.Length > 0 && separator == numberInfo.NumberDecimalSeparator[0])
			{
				separator = ';';
			}

			return separator;
		}
	}
}
