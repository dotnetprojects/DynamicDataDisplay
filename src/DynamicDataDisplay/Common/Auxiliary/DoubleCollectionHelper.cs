using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class DoubleCollectionHelper
	{
		public static DoubleCollection Create(params double[] collection)
		{
			return new DoubleCollection(collection);
		}
	}
}
