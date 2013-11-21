using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	public abstract class DateTimeTicksProviderBase : ITicksProvider<DateTime>
	{
		public event EventHandler Changed;
		protected void RaiseChanged()
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
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

		#region ITicksProvider<DateTime> Members

		public abstract ITicksInfo<DateTime> GetTicks(Range<DateTime> range, int ticksCount);
		public abstract int DecreaseTickCount(int ticksCount);
		public abstract int IncreaseTickCount(int ticksCount);
		public abstract ITicksProvider<DateTime> MinorProvider { get; }
		public abstract ITicksProvider<DateTime> MajorProvider { get; }

		#endregion
	}
}
