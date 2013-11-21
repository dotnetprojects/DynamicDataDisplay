using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	internal static class VectorExtensions
	{
		public static Point ToPoint(this Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}
	}
}
