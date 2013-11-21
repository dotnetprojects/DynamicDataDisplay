using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal sealed class TimeSpanToDoubleConversion
	{
		public TimeSpanToDoubleConversion(TimeSpan minSpan, TimeSpan maxSpan)
			: this(0, minSpan, 1, maxSpan)
		{ }

		public TimeSpanToDoubleConversion(double min, TimeSpan minSpan, double max, TimeSpan maxSpan)
		{
			this.min = min;
			this.length = max - min;
			this.ticksMin = minSpan.Ticks;
			this.ticksLength = maxSpan.Ticks - ticksMin;
		}

		private double min;
		private double length;
		private long ticksMin;
		private long ticksLength;

		internal TimeSpan FromDouble(double d)
		{
			double ratio = (d - min) / length;
			long ticks = (long)(ticksMin + ticksLength * ratio);

			ticks = MathHelper.Clamp(ticks, TimeSpan.MinValue.Ticks, TimeSpan.MaxValue.Ticks);

			return new TimeSpan(ticks);
		}

		internal double ToDouble(TimeSpan span)
		{
			double ratio = (span.Ticks - ticksMin) / (double)ticksLength;
			return min + ratio * length;
		}
	}

}
