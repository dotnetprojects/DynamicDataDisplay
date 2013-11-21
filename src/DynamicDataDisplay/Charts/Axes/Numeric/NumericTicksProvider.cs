using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Collections.ObjectModel;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
	/// <summary>
	/// Represents a ticks provider for <see cref="System.Double"/> values.
	/// </summary>
	public sealed class NumericTicksProvider : ITicksProvider<double>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NumericTicksProvider"/> class.
		/// </summary>
		public NumericTicksProvider()
		{
			minorProvider = new MinorNumericTicksProvider(this);
			minorProvider.Changed += minorProvider_Changed;
			minorProvider.Coeffs = new double[] { 0.3, 0.3, 0.3, 0.3, 0.6, 0.3, 0.3, 0.3, 0.3 };
		}

		private void minorProvider_Changed(object sender, EventArgs e)
		{
			Changed.Raise(this);
		}

		public event EventHandler Changed;
		private void RaiseChangedEvent()
		{
			Changed.Raise(this);
		}

		private double minStep = 0.0;
		/// <summary>
		/// Gets or sets the minimal step between ticks.
		/// </summary>
		/// <value>The min step.</value>
		public double MinStep
		{
			get { return minStep; }
			set
			{
				Verify.IsTrue(value >= 0.0, "value");
				if (minStep != value)
				{
					minStep = value;
					RaiseChangedEvent();
				}
			}
		}

		private double[] ticks;
		public ITicksInfo<double> GetTicks(Range<double> range, int ticksCount)
		{
			double start = range.Min;
			double finish = range.Max;

			double delta = finish - start;

			int log = (int)Math.Round(Math.Log10(delta));

			double newStart = RoundingHelper.Round(start, log);
			double newFinish = RoundingHelper.Round(finish, log);
			if (newStart == newFinish)
			{
				log--;
				newStart = RoundingHelper.Round(start, log);
				newFinish = RoundingHelper.Round(finish, log);
			}

			// calculating step between ticks
			double unroundedStep = (newFinish - newStart) / ticksCount;
			int stepLog = log;
			// trying to round step
			double step = RoundingHelper.Round(unroundedStep, stepLog);
			if (step == 0)
			{
				stepLog--;
				step = RoundingHelper.Round(unroundedStep, stepLog);
				if (step == 0)
				{
					// step will not be rounded if attempts to be rounded to zero.
					step = unroundedStep;
				}
			}

			if (step < minStep)
				step = minStep;

			if (step != 0.0)
			{
				ticks = CreateTicks(start, finish, step);
			}
			else
			{
				ticks = new double[] { };
			}

			TicksInfo<double> res = new TicksInfo<double> { Info = log, Ticks = ticks };

			return res;
		}

		private static double[] CreateTicks(double start, double finish, double step)
		{
			DebugVerify.Is(step != 0.0);

			double x = step * Math.Floor(start / step);

			if (x == x + step)
			{
				return new double[0];
			}

			List<double> res = new List<double>();

			double increasedFinish = finish + step * 1.05;
			while (x <= increasedFinish)
			{
				res.Add(x);
				DebugVerify.Is(res.Count < 2000);
				x += step;
			}
			return res.ToArray();
		}

		private static int[] tickCounts = new int[] { 20, 10, 5, 4, 2, 1 };

		public const int DefaultPreferredTicksCount = 10;

		public int DecreaseTickCount(int ticksCount)
		{
			return tickCounts.FirstOrDefault(tick => tick < ticksCount);
		}

		public int IncreaseTickCount(int ticksCount)
		{
			int newTickCount = tickCounts.Reverse().FirstOrDefault(tick => tick > ticksCount);
			if (newTickCount == 0)
				newTickCount = tickCounts[0];

			return newTickCount;
		}

		private readonly MinorNumericTicksProvider minorProvider;
		public ITicksProvider<double> MinorProvider
		{
			get
			{
				if (ticks != null)
				{
					minorProvider.SetRanges(ticks.GetPairs());
				}

				return minorProvider;
			}
		}

		public ITicksProvider<double> MajorProvider
		{
			get { return null; }
		}
	}
}
