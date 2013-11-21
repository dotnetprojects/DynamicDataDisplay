using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.ObjectModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts.NewAxis
{
	public sealed class DefaultDoubleTicksProvider : ITicksProvider<double>
	{
		public double[] GetTicks(Range<double> range, int preferredTicksCount)
		{
			double start = range.Min;
			double finish = range.Max;

			double delta = finish - start;

			int log = (int)Math.Round(Math.Log10(delta));

			double newStart = Round(start, log);
			double newFinish = Round(finish, log);
			if (newStart == newFinish)
			{
				log--;
				newStart = Round(start, log);
				newFinish = Round(finish, log);
			}

			double step = (newFinish - newStart) / preferredTicksCount;

			//double[] ticks = CreateTicks(newStart, newFinish, preferredTicksCount);
			double[] ticks = CreateTicks(start, finish, step);
			return ticks;
		}

		protected static double[] CreateTicks(double start, double finish, double step)
		{
			double x = step * (Math.Floor(start / step) + 1);
			List<double> res = new List<double>();
			while (x <= finish)
			{
				res.Add(x);
				x += step;
			}
			return res.ToArray();
		}

		//private static double[] CreateTicks(double start, double finish, int tickCount)
		//{
		//    double[] ticks = new double[tickCount];
		//    if (tickCount == 0)
		//        return ticks;

		//    DebugVerify.Is(tickCount > 0);

		//    double delta = (finish - start) / (tickCount - 1);

		//    for (int i = 0; i < tickCount; i++)
		//    {
		//        ticks[i] = start + i * delta;
		//    }

		//    return ticks;
		//}

		private static double Round(double number, int rem)
		{
			if (rem <= 0)
			{
				return Math.Round(number, -rem);
			}
			else
			{
				double pow = Math.Pow(10, rem - 1);
				double val = pow * Math.Round(number / Math.Pow(10, rem - 1));
				return val;
			}
		}

		private static ReadOnlyCollection<int> TickCount =
			new ReadOnlyCollection<int>(new int[] { 20, 10, 5, 4, 2, 1 });

		public const int DefaultPreferredTicksCount = 10;

		public int DecreaseTickCount(int tickCount)
		{
			return TickCount.FirstOrDefault(tick => tick < tickCount);
		}

		public int IncreaseTickCount(int tickCount) {
			int newTickCount = TickCount.Reverse().FirstOrDefault(tick => tick > tickCount);
			if (newTickCount == 0)
				newTickCount = TickCount[0];

			return newTickCount;
		}
	}
}
