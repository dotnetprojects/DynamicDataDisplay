using System.Windows;
using System;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class PointExtensions
	{
		public static Vector ToVector(this Point pt)
		{
			return new Vector(pt.X, pt.Y);
		}

		public static bool IsFinite(this Point pt)
		{
			return pt.X.IsFinite() && pt.Y.IsFinite();
		}
	}
}
