using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal static class DefaultTicksProvider
	{
		internal static readonly int DefaultTicksCount = 10;

		internal static ITicksInfo<T> GetTicks<T>(this ITicksProvider<T> provider, Range<T> range)
		{
			return provider.GetTicks(range, DefaultTicksCount);
		}
	}
}
