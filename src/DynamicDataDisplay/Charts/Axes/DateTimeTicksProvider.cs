using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewAxis
{
	public class DateTimeTicksProvider : DateTimeTicksProviderBase
	{
		private static readonly Dictionary<DifferenceIn, ITicksProvider<DateTime>> providers =
			new Dictionary<DifferenceIn, ITicksProvider<DateTime>>();

		static DateTimeTicksProvider()
		{
			providers.Add(DifferenceIn.Year, new YearProvider());
			providers.Add(DifferenceIn.Month, new MonthProvider());
			providers.Add(DifferenceIn.Day, new DayProvider());
			providers.Add(DifferenceIn.Hour, new HourProvider());
			providers.Add(DifferenceIn.Minute, new MinuteProvider());
			providers.Add(DifferenceIn.Second, new SecondProvider());
		}

		private DifferenceIn diff;
		/// <summary>
		/// Gets the ticks.
		/// </summary>
		/// <param name="range">The range.</param>
		/// <param name="ticksCount">The ticks count.</param>
		/// <returns></returns>
		public override ITicksInfo<DateTime> GetTicks(Range<DateTime> range, int ticksCount)
		{
			Verify.Is(ticksCount > 0);

			DateTime start = range.Min;
			DateTime end = range.Max;
			TimeSpan length = end - start;

			diff = GetDifference(length);

			TicksInfo<DateTime> res = new TicksInfo<DateTime> { Info = diff };
			if (providers.ContainsKey(diff))
			{
				ITicksInfo<DateTime> result = providers[diff].GetTicks(range, ticksCount);
				DateTime[] mayorTicks = result.Ticks;

				res.Ticks = mayorTicks;

				DifferenceIn lowerDiff = DifferenceIn.Year;
				// todo разобраться с minor ticks
				bool lowerDiffExists = TryGetLowerDiff(diff, out lowerDiff);
				if (lowerDiffExists && providers.ContainsKey(lowerDiff))
				{
					var minorTicks = result.Ticks.GetPairs().Select(r => ((IMinorTicksProvider<DateTime>)providers[lowerDiff]).CreateTicks(r)).
						SelectMany(m => m).ToArray();

					res.MinorTicks = minorTicks;
				}
				return res;
			}


			DateTime newStart = RoundDown(start, diff);
			DateTime newEnd = RoundUp(end, diff);

			DebugVerify.Is(newStart <= start);

			List<DateTime> resultTicks = new List<DateTime>();
			DateTime dt = newStart;
			do
			{
				resultTicks.Add(dt);
				dt = Shift(dt, diff);
			} while (dt <= newEnd);

			while (resultTicks.Count > ticksCount)
			{
				var res2 = resultTicks;
				resultTicks = res2.Where((date, i) => i % 2 == 0).ToList();
			}

			res.Ticks = resultTicks.ToArray();

			return res;
		}

		/// <summary>
		/// Tries the get lower diff.
		/// </summary>
		/// <param name="diff">The diff.</param>
		/// <param name="lowerDiff">The lower diff.</param>
		/// <returns></returns>
		private static bool TryGetLowerDiff(DifferenceIn diff, out DifferenceIn lowerDiff)
		{
			lowerDiff = diff;

			int code = (int)diff;
			bool res = code > 0;
			if (res)
			{
				lowerDiff = (DifferenceIn)(code - 1);
			}
			return res;
		}

		/// <summary>
		/// Decreases the tick count.
		/// </summary>
		/// <param name="tickCount">The tick count.</param>
		/// <returns></returns>
		public override int DecreaseTickCount(int tickCount)
		{
			if (providers.ContainsKey(diff))
				return providers[diff].DecreaseTickCount(tickCount);

			int res = tickCount / 2;
			if (res < 2) res = 2;
			return res;
		}

		/// <summary>
		/// Increases the tick count.
		/// </summary>
		/// <param name="tickCount">The tick count.</param>
		/// <returns></returns>
		public override int IncreaseTickCount(int tickCount)
		{
			DebugVerify.Is(tickCount < 2000);

			if (providers.ContainsKey(diff))
				return providers[diff].IncreaseTickCount(tickCount);

			return tickCount * 2;
		}
	}

	public enum DifferenceIn
	{
		Year = 7,
		Month = 6,
		Day = 5,
		Hour = 4,
		Minute = 3,
		Second = 2,
		Millisecond = 1
	}

	internal static class DateTimeArrayExt
	{
		[Obsolete("Works wrongly", true)]
		internal static DateTime[] Clip(this DateTime[] array, DateTime start, DateTime end)
		{
			if (start > end)
			{
				DateTime temp = start;
				start = end;
				end = temp;
			}

			int startIndex = array.GetIndex(start);
			int endIndex = array.GetIndex(end) + 1;
			DateTime[] res = new DateTime[endIndex - startIndex];
			Array.Copy(array, startIndex, res, 0, res.Length);

			return res;
		}

		internal static int GetIndex(this DateTime[] array, DateTime value)
		{
			for (int i = 0; i < array.Length - 1; i++)
			{
				if (array[i] <= value && value < array[i + 1])
					return i;
			}

			return array.Length - 1;
		}
	}

	internal abstract class DatePeriodTicksProvider : DateTimeTicksProviderBase, IMinorTicksProvider<DateTime>
	{
		protected DatePeriodTicksProvider()
		{
			tickCounts = GetTickCountsCore();
			difference = GetDifferenceCore();
		}

		protected DifferenceIn difference;
		protected abstract DifferenceIn GetDifferenceCore();

		protected abstract int[] GetTickCountsCore();
		protected int[] tickCounts = { };

		public sealed override int DecreaseTickCount(int ticksCount)
		{
			if (ticksCount > tickCounts[0]) return tickCounts[0];

			for (int i = 0; i < tickCounts.Length; i++)
				if (ticksCount > tickCounts[i])
					return tickCounts[i];

			return tickCounts.Last();
		}

		public sealed override int IncreaseTickCount(int ticksCount)
		{
			if (ticksCount >= tickCounts[0]) return tickCounts[0];

			for (int i = tickCounts.Length - 1; i >= 0; i--)
				if (ticksCount < tickCounts[i])
					return tickCounts[i];

			return tickCounts.Last();
		}

		protected abstract int GetSpecificValue(DateTime start, DateTime dt);
		protected abstract DateTime GetStart(DateTime start, int value, int step);
		protected abstract bool IsMinDate(DateTime dt);
		protected abstract DateTime AddStep(DateTime dt, int step);

		public sealed override ITicksInfo<DateTime> GetTicks(Range<DateTime> range, int ticksCount)
		{
			DateTime start = range.Min;
			DateTime end = range.Max;
			TimeSpan length = end - start;

			bool isPositive = length.Ticks > 0;
			DifferenceIn diff = difference;

			DateTime newStart = isPositive ? RoundDown(start, diff) : SafelyRoundUp(start);
			DateTime newEnd = isPositive ? SafelyRoundUp(end) : RoundDown(end, diff);

			RoundingInfo bounds = RoundHelper.CreateRoundedRange(GetSpecificValue(newStart, newStart), GetSpecificValue(newStart, newEnd));

			int delta = (int)(bounds.Max - bounds.Min);
			if (delta == 0)
				return new TicksInfo<DateTime> { Ticks = new DateTime[] { newStart } };

			int step = delta / ticksCount;

			if (step == 0) step = 1;

			DateTime tick = GetStart(newStart, (int)bounds.Min, step);
			bool isMinDateTime = IsMinDate(tick) && step != 1;
			if (isMinDateTime)
				step--;

			List<DateTime> ticks = new List<DateTime>();
			DateTime finishTick = AddStep(range.Max, step);
			while (tick < finishTick)
			{
				ticks.Add(tick);
				tick = AddStep(tick, step);
				if (isMinDateTime)
				{
					isMinDateTime = false;
					step++;
				}
			}

			TicksInfo<DateTime> res = new TicksInfo<DateTime> { Ticks = ticks.ToArray(), Info = diff };
			return res;
		}

		private DateTime SafelyRoundUp(DateTime dt)
		{
			if (AddStep(dt, 1) == DateTime.MaxValue)
				return DateTime.MaxValue;

			return RoundUp(dt, difference);
		}

		#region IMinorTicksProvider<DateTime> Members

		public MinorTickInfo<DateTime>[] CreateTicks(Range<DateTime> range)
		{
			int tickCount = tickCounts[1];
			ITicksInfo<DateTime> ticks = GetTicks(range, tickCount);

			MinorTickInfo<DateTime>[] res = ticks.Ticks.
				Select(dt => new MinorTickInfo<DateTime>(0.5, dt)).ToArray();

			return res;
		}

		#endregion
	}

	internal class YearProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Year;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 20, 10, 5, 4, 2, 1 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return dt.Year;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			int year = start.Year;
			int newYear = (year / step) * step;
			if (newYear == 0) newYear = 1;

			return new DateTime(newYear, 1, 1);
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return dt.Year == DateTime.MinValue.Year;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			if (dt.Year + step > DateTime.MaxValue.Year)
				return DateTime.MaxValue;

			return dt.AddYears(step);
		}
	}

	internal class MonthProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Month;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 12, 6, 4, 3, 2, 1 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return dt.Month + (dt.Year - start.Year) * 12;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			return new DateTime(start.Year, 1, 1);
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return dt.Month == DateTime.MinValue.Month;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			return dt.AddMonths(step);
		}
	}

	internal class DayProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Day;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 30, 15, 10, 5, 2, 1 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return (dt - start).Days;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			return start.Date;
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return dt.Day == 1;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			return dt.AddDays(step);
		}
	}

	internal class HourProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Hour;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 24, 12, 6, 4, 3, 2, 1 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return (dt - start).Hours;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			return start.Date;//.AddHours(start.Hour);
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return false;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			return dt.AddHours(step);
		}
	}

	internal class MinuteProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Minute;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 60, 30, 20, 15, 10, 5, 4, 3, 2 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return (dt - start).Minutes;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			return start.Date.AddHours(start.Hour);
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return false;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			return dt.AddMinutes(step);
		}
	}

	internal class SecondProvider : DatePeriodTicksProvider
	{
		protected override DifferenceIn GetDifferenceCore()
		{
			return DifferenceIn.Second;
		}

		protected override int[] GetTickCountsCore()
		{
			return new int[] { 60, 30, 20, 15, 10, 5, 4, 3, 2 };
		}

		protected override int GetSpecificValue(DateTime start, DateTime dt)
		{
			return (dt - start).Seconds;
		}

		protected override DateTime GetStart(DateTime start, int value, int step)
		{
			return start.Date.AddHours(start.Hour).AddMinutes(start.Minute);
		}

		protected override bool IsMinDate(DateTime dt)
		{
			return false;
		}

		protected override DateTime AddStep(DateTime dt, int step)
		{
			return dt.AddSeconds(step);
		}
	}
}
