using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay
{
	public static class DataDomains
	{
		private static readonly DataRect xPositive = DataRect.FromPoints(Double.Epsilon, Double.MinValue / 2, Double.MaxValue, Double.MaxValue / 2);
		public static DataRect XPositive
		{
			get { return xPositive; }
		}

		private static readonly DataRect yPositive = DataRect.FromPoints(Double.MinValue / 2, Double.Epsilon, Double.MaxValue / 2, Double.MaxValue);
		public static DataRect YPositive
		{
			get { return yPositive; }
		}

		private static readonly DataRect xyPositive = DataRect.FromPoints(Double.Epsilon, Double.Epsilon, Double.MaxValue, Double.MaxValue);
		public static DataRect XYPositive
		{
			get { return xyPositive; }
		} 
	}
}
