using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public class TimeSpanTicksProvider : TimeTicksProviderBase<TimeSpan>
	{
		static TimeSpanTicksProvider()
		{
			Providers.Add(DifferenceIn.Year, new DayTimeSpanProvider());
			Providers.Add(DifferenceIn.Month, new DayTimeSpanProvider());
			Providers.Add(DifferenceIn.Day, new DayTimeSpanProvider());
			Providers.Add(DifferenceIn.Hour, new HourTimeSpanProvider());
			Providers.Add(DifferenceIn.Minute, new MinuteTimeSpanProvider());
			Providers.Add(DifferenceIn.Second, new SecondTimeSpanProvider());
			Providers.Add(DifferenceIn.Millisecond, new MillisecondTimeSpanProvider());

			MinorProviders.Add(DifferenceIn.Year, new MinorTimeSpanTicksProvider(new DayTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Month, new MinorTimeSpanTicksProvider(new DayTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Day, new MinorTimeSpanTicksProvider(new DayTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Hour, new MinorTimeSpanTicksProvider(new HourTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Minute, new MinorTimeSpanTicksProvider(new MinuteTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Second, new MinorTimeSpanTicksProvider(new SecondTimeSpanProvider()));
			MinorProviders.Add(DifferenceIn.Millisecond, new MinorTimeSpanTicksProvider(new MillisecondTimeSpanProvider()));
		}

		protected override TimeSpan GetDifference(TimeSpan start, TimeSpan end)
		{
			return end - start;
		}
	}
}
