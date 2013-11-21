using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewAxis
{
	public abstract class DateTimeTicksProviderBase : ITicksProvider<DateTime>
	{
		public abstract ITicksInfo<DateTime> GetTicks(Range<DateTime> range, int ticksCount);

		public abstract int DecreaseTickCount(int ticksCount);

		public abstract int IncreaseTickCount(int ticksCount);

		protected static DifferenceIn GetDifference(TimeSpan span)
		{
			// for negative time spans
			span = span.Duration();

			DifferenceIn diff;
			if (span.Days > 365)
				diff = DifferenceIn.Year;
			else if (span.Days > 30)
				diff = DifferenceIn.Month;
			else if (span.Days > 0)
				diff = DifferenceIn.Day;
			else if (span.Hours > 0)
				diff = DifferenceIn.Hour;
			else if (span.Minutes > 0)
				diff = DifferenceIn.Minute;
			else if (span.Seconds > 0)
				diff = DifferenceIn.Second;
			else
				diff = DifferenceIn.Millisecond;

			return diff;
		}

		protected static DateTime Shift(DateTime dateTime, DifferenceIn diff)
		{
			DateTime res = dateTime;

			switch (diff)
			{
				case DifferenceIn.Year:
					res = res.AddYears(1);
					break;
				case DifferenceIn.Month:
					res = res.AddMonths(1);
					break;
				case DifferenceIn.Day:
					res = res.AddDays(1);
					break;
				case DifferenceIn.Hour:
					res = res.AddHours(1);
					break;
				case DifferenceIn.Minute:
					res = res.AddMinutes(1);
					break;
				case DifferenceIn.Second:
					res = res.AddSeconds(1);
					break;
				case DifferenceIn.Millisecond:
					res = res.AddMilliseconds(1);
					break;
				default:
					break;
			}

			return res;
		}

		protected static DateTime RoundDown(DateTime dateTime, DifferenceIn diff)
		{
			DateTime res = dateTime;

			switch (diff)
			{
				case DifferenceIn.Year:
					res = new DateTime(dateTime.Year, 1, 1);
					break;
				case DifferenceIn.Month:
					res = new DateTime(dateTime.Year, dateTime.Month, 1);
					break;
				case DifferenceIn.Day:
					res = dateTime.Date;
					break;
				case DifferenceIn.Hour:
					res = dateTime.Date.AddHours(dateTime.Hour);
					break;
				case DifferenceIn.Minute:
					res = dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute);
					break;
				case DifferenceIn.Second:
					res = dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute).AddSeconds(dateTime.Second);
					break;
				case DifferenceIn.Millisecond:
					res = dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute).AddSeconds(dateTime.Second).AddMilliseconds(dateTime.Millisecond);
					break;
				default:
					break;
			}

			DebugVerify.Is(res <= dateTime);

			return res;
		}

		protected static DateTime RoundUp(DateTime dateTime, DifferenceIn diff)
		{
			DateTime res = RoundDown(dateTime, diff);

			switch (diff)
			{
				case DifferenceIn.Year:
					res = res.AddYears(1);
					break;
				case DifferenceIn.Month:
					res = res.AddMonths(1);
					break;
				case DifferenceIn.Day:
					res = res.AddDays(1);
					break;
				case DifferenceIn.Hour:
					res = res.AddHours(1);
					break;
				case DifferenceIn.Minute:
					res = res.AddMinutes(1);
					break;
				case DifferenceIn.Second:
					res = res.AddSeconds(1);
					break;
				case DifferenceIn.Millisecond:
					res = res.AddMilliseconds(1);
					break;
				default:
					break;
			}

			return res;
		}
	}
}
