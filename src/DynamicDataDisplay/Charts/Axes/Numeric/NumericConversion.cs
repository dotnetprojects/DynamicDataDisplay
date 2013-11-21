using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes.Numeric
{
	internal sealed class NumericConversion
	{
		private readonly double min;
		private readonly double length;
		private readonly double minValue;
		private readonly double valueLength;

		public NumericConversion(double min, double minValue, double max, double maxValue)
		{
			this.min = min;
			this.length = max - min;

			this.minValue = minValue;
			this.valueLength = maxValue - minValue;
		}

		public double FromDouble(double value)
		{
			double ratio = (value - min) / length;

			return minValue + ratio * valueLength;
		}

		public double ToDouble(double value)
		{
			double ratio = (value - minValue) / valueLength;

			return min + length * ratio;
		}
	}
}
