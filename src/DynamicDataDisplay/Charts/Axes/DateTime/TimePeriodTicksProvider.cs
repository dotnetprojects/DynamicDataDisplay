using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	internal abstract class TimePeriodTicksProvider<T> : ITicksProvider<T>
	{
		public event EventHandler Changed;
		protected void RaiseChanged()
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}

		protected abstract T RoundUp(T time, DifferenceIn diff);
		protected abstract T RoundDown(T time, DifferenceIn diff);

		private bool differenceInited = false;
		private DifferenceIn difference;
		protected DifferenceIn Difference
		{
			get
			{
				if (!differenceInited)
				{
					difference = GetDifferenceCore();
					differenceInited = true;
				}
				return difference;
			}
		}
		protected abstract DifferenceIn GetDifferenceCore();

		private int[] tickCounts = null;
		protected int[] TickCounts
		{
			get
			{
				if (tickCounts == null)
					tickCounts = GetTickCountsCore();
				return tickCounts;
			}
		}
		protected abstract int[] GetTickCountsCore();

		public int DecreaseTickCount(int ticksCount)
		{
			if (ticksCount > TickCounts[0]) return TickCounts[0];

			for (int i = 0; i < TickCounts.Length; i++)
				if (ticksCount > TickCounts[i])
					return TickCounts[i];

			return TickCounts.Last();
		}

		public int IncreaseTickCount(int ticksCount)
		{
			if (ticksCount >= TickCounts[0]) return TickCounts[0];

			for (int i = TickCounts.Length - 1; i >= 0; i--)
				if (ticksCount < TickCounts[i])
					return TickCounts[i];

			return TickCounts.Last();
		}

		protected abstract int GetSpecificValue(T start, T dt);
		protected abstract T GetStart(T start, int value, int step);
		protected abstract bool IsMinDate(T dt);
		protected abstract T AddStep(T dt, int step);

		public ITicksInfo<T> GetTicks(Range<T> range, int ticksCount)
		{
			T start = range.Min;
			T end = range.Max;
			DifferenceIn diff = Difference;
			start = RoundDown(start, end);
			end = RoundUp(start, end);

			RoundingInfo bounds = RoundingHelper.CreateRoundedRange(
				GetSpecificValue(start, start),
				GetSpecificValue(start, end));

			int delta = (int)(bounds.Max - bounds.Min);
			if (delta == 0)
				return new TicksInfo<T> { Ticks = new T[] { start } };

			int step = delta / ticksCount;

			if (step == 0) step = 1;

			T tick = GetStart(start, (int)bounds.Min, step);
			bool isMinDateTime = IsMinDate(tick) && step != 1;
			if (isMinDateTime)
				step--;

			List<T> ticks = new List<T>();
			T finishTick = AddStep(range.Max, step);
			while (Continue(tick, finishTick))
			{
				ticks.Add(tick);
				tick = AddStep(tick, step);
				if (isMinDateTime)
				{
					isMinDateTime = false;
					step++;
				}
			}

			ticks = Trim(ticks, range);

			TicksInfo<T> res = new TicksInfo<T> { Ticks = ticks.ToArray(), Info = diff };
			return res;
		}

		protected abstract bool Continue(T current, T end);

		protected abstract T RoundUp(T start, T end);

		protected abstract T RoundDown(T start, T end);

		protected abstract List<T> Trim(List<T> ticks, Range<T> range);

		public ITicksProvider<T> MinorProvider
		{
			get { throw new NotSupportedException(); }
		}

		public ITicksProvider<T> MajorProvider
		{
			get { throw new NotSupportedException(); }
		}
	}

	internal abstract class DatePeriodTicksProvider : TimePeriodTicksProvider<DateTime>
	{
		protected sealed override bool Continue(DateTime current, DateTime end)
		{
			return current < end;
		}

		protected sealed override List<DateTime> Trim(List<DateTime> ticks, Range<DateTime> range)
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

			List<DateTime> res = new List<DateTime>(endIndex - startIndex + 1);
			for (int i = startIndex; i <= endIndex; i++)
			{
				res.Add(ticks[i]);
			}

			return res;
		}

		protected sealed override DateTime RoundUp(DateTime start, DateTime end)
		{
			bool isPositive = (end - start).Ticks > 0;
			return isPositive ? SafelyRoundUp(end) : RoundDown(end, Difference);
		}

		private DateTime SafelyRoundUp(DateTime dt)
		{
			if (AddStep(dt, 1) == DateTime.MaxValue)
				return DateTime.MaxValue;

			return RoundUp(dt, Difference);
		}

		protected sealed override DateTime RoundDown(DateTime start, DateTime end)
		{
			bool isPositive = (end - start).Ticks > 0;
			return isPositive ? RoundDown(start, Difference) : SafelyRoundUp(start);
		}

		protected sealed override DateTime RoundDown(DateTime time, DifferenceIn diff)
		{
			DateTime res = time;

			switch (diff)
			{
				case DifferenceIn.Year:
					res = new DateTime(time.Year, 1, 1);
					break;
				case DifferenceIn.Month:
					res = new DateTime(time.Year, time.Month, 1);
					break;
				case DifferenceIn.Day:
					res = time.Date;
					break;
				case DifferenceIn.Hour:
					res = time.Date.AddHours(time.Hour);
					break;
				case DifferenceIn.Minute:
					res = time.Date.AddHours(time.Hour).AddMinutes(time.Minute);
					break;
				case DifferenceIn.Second:
					res = time.Date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
					break;
				case DifferenceIn.Millisecond:
					res = time.Date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second).AddMilliseconds(time.Millisecond);
					break;
				default:
					break;
			}

			DebugVerify.Is(res <= time);

			return res;
		}

		protected override DateTime RoundUp(DateTime dateTime, DifferenceIn diff)
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
