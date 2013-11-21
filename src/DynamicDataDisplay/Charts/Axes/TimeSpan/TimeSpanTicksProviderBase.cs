using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal abstract class TimeSpanTicksProviderBase : TimePeriodTicksProvider<TimeSpan>
	{
		protected sealed override bool Continue(TimeSpan current, TimeSpan end)
		{
			return current < end;
		}

		protected sealed override TimeSpan RoundDown(TimeSpan start, TimeSpan end)
		{
			return RoundDown(start, Difference);
		}

		protected sealed override TimeSpan RoundUp(TimeSpan start, TimeSpan end)
		{
			return RoundUp(end, Difference);
		}

		protected static TimeSpan Shift(TimeSpan span, DifferenceIn diff)
		{
			TimeSpan res = span;

			TimeSpan shift = new TimeSpan();
			switch (diff)
			{
				case DifferenceIn.Year:
				case DifferenceIn.Month:
				case DifferenceIn.Day:
					shift = TimeSpan.FromDays(1);
					break;
				case DifferenceIn.Hour:
					shift = TimeSpan.FromHours(1);
					break;
				case DifferenceIn.Minute:
					shift = TimeSpan.FromMinutes(1);
					break;
				case DifferenceIn.Second:
					shift = TimeSpan.FromSeconds(1);
					break;
				case DifferenceIn.Millisecond:
					shift = TimeSpan.FromMilliseconds(1);
					break;
				default:
					break;
			}

			res = res.Add(shift);
			return res;
		}

		protected sealed override TimeSpan RoundDown(TimeSpan timeSpan, DifferenceIn diff)
		{
			TimeSpan res = timeSpan;

			if (timeSpan.Ticks < 0)
			{
				res = RoundUp(timeSpan.Duration(), diff).Negate();
			}
			else
			{
				switch (diff)
				{
					case DifferenceIn.Year:
					case DifferenceIn.Month:
					case DifferenceIn.Day:
						res = TimeSpan.FromDays(timeSpan.Days);
						break;
					case DifferenceIn.Hour:
						res = TimeSpan.FromDays(timeSpan.Days).
							Add(TimeSpan.FromHours(timeSpan.Hours));
						break;
					case DifferenceIn.Minute:
						res = TimeSpan.FromDays(timeSpan.Days).
							Add(TimeSpan.FromHours(timeSpan.Hours)).
							Add(TimeSpan.FromMinutes(timeSpan.Minutes));
						break;
					case DifferenceIn.Second:
						res = TimeSpan.FromDays(timeSpan.Days).
							Add(TimeSpan.FromHours(timeSpan.Hours)).
							Add(TimeSpan.FromMinutes(timeSpan.Minutes)).
							Add(TimeSpan.FromSeconds(timeSpan.Seconds));
						break;
					case DifferenceIn.Millisecond:
						res = timeSpan;
						break;
					default:
						break;
				}
			}

			return res;
		}

		protected sealed override TimeSpan RoundUp(TimeSpan dateTime, DifferenceIn diff)
		{
			TimeSpan res = RoundDown(dateTime, diff);
			res = Shift(res, diff);

			return res;
		}

		protected override List<TimeSpan> Trim(List<TimeSpan> ticks, Range<TimeSpan> range)
		{
			int startIndex = 0;
			for (int i = 0; i < ticks.Count - 1; i++)
			{
				if (ticks[i] <= range.Min && range.Min <= ticks[i + 1])
				{
					startIndex = i;
					break;
				}
			}

			int endIndex = ticks.Count - 1;
			for (int i = ticks.Count - 1; i >= 1; i--)
			{
				if (ticks[i] >= range.Max && range.Max > ticks[i - 1])
				{
					endIndex = i;
					break;
				}
			}

			List<TimeSpan> res = new List<TimeSpan>(endIndex - startIndex + 1);
			for (int i = startIndex; i <= endIndex; i++)
			{
				res.Add(ticks[i]);
			}

			return res;
		}

		protected sealed override bool IsMinDate(TimeSpan dt)
		{
			return false;
		}
	}

	internal sealed class DayTimeSpanProvider : TimeSpanTicksProviderBase
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Day;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 20, 10, 5, 2, 1 };
		}

		protected override int GetSpecificValue(TimeSpan start, TimeSpan dt)
		{
			return (dt - start).Days;
		}

		protected override TimeSpan GetStart(TimeSpan start, int value, int step)
		{
			double days = start.TotalDays;
			double newDays = ((int)(days / step)) * step;
			if (newDays > days) { 
				newDays -= step;
			}
			return TimeSpan.FromDays(newDays);
			//return TimeSpan.FromDays(start.Days);
		}

		protected override TimeSpan AddStep(TimeSpan dt, int step)
		{
			return dt.Add(TimeSpan.FromDays(step));
		}
	}

	internal sealed class HourTimeSpanProvider : TimeSpanTicksProviderBase
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Hour;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 24, 12, 6, 4, 3, 2, 1 };
		}

		protected override int GetSpecificValue(TimeSpan start, TimeSpan dt)
		{
			return (int)(dt - start).TotalHours;
		}

		protected override TimeSpan GetStart(TimeSpan start, int value, int step)
		{
			double hours = start.TotalHours;
			double newHours = ((int)(hours / step)) * step;
			if (newHours > hours)
			{
				newHours -= step;
			}
			return TimeSpan.FromHours(newHours);
			//return TimeSpan.FromDays(start.Days);
		}

		protected override TimeSpan AddStep(TimeSpan dt, int step)
		{
			return dt.Add(TimeSpan.FromHours(step));
		}
	}

	internal sealed class MinuteTimeSpanProvider : TimeSpanTicksProviderBase
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Minute;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 60, 30, 20, 15, 10, 5, 4, 3, 2 };
		}

		protected override int GetSpecificValue(TimeSpan start, TimeSpan dt)
		{
			return (int)(dt - start).TotalMinutes;
		}

		protected override TimeSpan GetStart(TimeSpan start, int value, int step)
		{
			double minutes = start.TotalMinutes;
			double newMinutes = ((int)(minutes / step)) * step;
			if (newMinutes > minutes)
			{
				newMinutes -= step;
			}

			return TimeSpan.FromMinutes(newMinutes);
		}

		protected override TimeSpan AddStep(TimeSpan dt, int step)
		{
			return dt.Add(TimeSpan.FromMinutes(step));
		}
	}

	internal sealed class SecondTimeSpanProvider : TimeSpanTicksProviderBase
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Second;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 60, 30, 20, 15, 10, 5, 4, 3, 2 };
		}

		protected override int GetSpecificValue(TimeSpan start, TimeSpan dt)
		{
			return (int)(dt - start).TotalSeconds;
		}

		protected override TimeSpan GetStart(TimeSpan start, int value, int step)
		{
			double seconds = start.TotalSeconds;
			double newSeconds = ((int)(seconds / step)) * step;
			if (newSeconds > seconds) {
				newSeconds -= step;
			}

			return TimeSpan.FromSeconds(newSeconds);
			//return new TimeSpan(start.Days, start.Hours, start.Minutes, 0);
		}

		protected override TimeSpan AddStep(TimeSpan dt, int step)
		{
			return dt.Add(TimeSpan.FromSeconds(step));
		}
	}

	internal sealed class MillisecondTimeSpanProvider : TimeSpanTicksProviderBase
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Millisecond;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 100, 50, 40, 25, 20, 10, 5, 4, 2 };
		}

		protected override int GetSpecificValue(TimeSpan start, TimeSpan dt)
		{
			return (int)(dt - start).TotalMilliseconds;
		}

		protected override TimeSpan GetStart(TimeSpan start, int value, int step)
		{
			double millis = start.TotalMilliseconds;
			double newMillis = ((int)(millis / step)) * step;
			if (newMillis > millis) {
				newMillis -= step;
			}

			return TimeSpan.FromMilliseconds(newMillis);
			//return start;
		}

		protected override TimeSpan AddStep(TimeSpan dt, int step)
		{
			return dt.Add(TimeSpan.FromMilliseconds(step));
		}
	}
}
