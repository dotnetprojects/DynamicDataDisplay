using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents axis with values of type TimeSpan.
	/// </summary>
	public class TimeSpanAxis : AxisBase<TimeSpan>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSpanAxis"/> class with default values conversion.
		/// </summary>
		public TimeSpanAxis()
			: base(new TimeSpanAxisControl(),
				DoubleToTimeSpan, TimeSpanToDouble)
		{ }

		private static readonly long minTicks = TimeSpan.MinValue.Ticks;
		private static readonly long maxTicks = TimeSpan.MaxValue.Ticks;
		private static TimeSpan DoubleToTimeSpan(double value)
		{
			long ticks = (long)(value * 10000000000L);

			// todo should we throw an exception if number of ticks is too big or small?
			if (ticks < minTicks)
				ticks = minTicks;
			else if (ticks > maxTicks)
				ticks = maxTicks;

			return new TimeSpan(ticks);
		}

		private static double TimeSpanToDouble(TimeSpan time)
		{
			return time.Ticks / 10000000000.0;
		}

		/// <summary>
		/// Sets conversions of axis - functions used to convert values of axis type to and from double values of viewport.
		/// Sets both ConvertToDouble and ConvertFromDouble properties.
		/// </summary>
		/// <param name="min">The minimal viewport value.</param>
		/// <param name="minValue">The value of axis type, corresponding to minimal viewport value.</param>
		/// <param name="max">The maximal viewport value.</param>
		/// <param name="maxValue">The value of axis type, corresponding to maximal viewport value.</param>
		public override void SetConversion(double min, TimeSpan minValue, double max, TimeSpan maxValue)
		{
			var conversion = new TimeSpanToDoubleConversion(min, minValue, max, maxValue);

			ConvertToDouble = conversion.ToDouble;
			ConvertFromDouble = conversion.FromDouble;
		}
	}
}
